using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Authentication;
using Shared.Helpers;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOtpService _otp;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _email;
        private readonly Jwt _jwt;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
        public AuthenticationService(UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> RoleManager,
                                SignInManager<ApplicationUser> signInManager,
                                IUrlHelperFactory urlHelperFactory,
                                IHttpContextAccessor httpContextAccessor,
                                IOptions<Jwt> jwt, IOtpService otp,
                                IMemoryCache cache, IEmailService email, IUnitOfWork unitOfWork,
                                IPasswordValidator<ApplicationUser> passwordValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
            _jwt = jwt.Value;
            _otp = otp;
            _email = email;
            _cache = cache;
            _RoleManager = RoleManager;
            _passwordValidator = passwordValidator;
        }
        public async Task<AuthenticationModelDto> RegisterAsync(RegisterModelDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser is not null)
            {
                return new AuthenticationModelDto
                {
                    Message = "User with this email already exists",
                };
            }

            if (model.Username is not null)
            {
                var existingUserNameEN = await _userManager.FindByNameAsync(model.Username);
                if (existingUserNameEN is not null)
                {
                    return new AuthenticationModelDto
                    {
                        Message = "User with this UserName already exists",
                    };
                }
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PasswordHash = model.Password,
                EmailConfirmed = true,
            };
            var passwordValidationResult = await _passwordValidator.ValidateAsync(_userManager, null, model.Password);

            if (!passwordValidationResult.Succeeded)
            {
                var errors = string.Join(", ", passwordValidationResult.Errors.Select(e => e.Description));
                return new AuthenticationModelDto
                {
                    Message = $"Password is not valid: {errors}"
                };
            }

            _cache.Set($"{model.Email}_PendingUser", user, TimeSpan.FromMinutes(10));

            var otpCode = await _otp.GenerateOtpAsync(model.Email, true);

            var emailResponse = await _email.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code for verification is: {otpCode}");

            if (!emailResponse.IsAuthenticated)
            {
                return emailResponse;
            }

            return new AuthenticationModelDto
            {
                Email = user.Email,
                IsAuthenticated = false,
                Message = "OTP sent to your email. Please verify to complete registration."
            };
        }
        public async Task<AuthenticationModelDto> SendOtpForRegister(VerifyOtpRequestModelDto verifyOtpRegister)
        {
            AuthenticationModelDto authenticationModel = new AuthenticationModelDto();

            if (!_cache.TryGetValue($"{verifyOtpRegister.Email}_Verified", out OtpData storedOtp) || storedOtp.Code != verifyOtpRegister.Otp)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Invalid or expired OTP";
                return authenticationModel;
            }

            if (!_cache.TryGetValue($"{verifyOtpRegister.Email}_PendingUser", out ApplicationUser user))
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "No pending registration found or expired.";
                return authenticationModel;
            }

            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return new AuthenticationModelDto { Message = errors };
            }

            await _userManager.AddToRoleAsync(user,"User");


            _cache.Remove($"{verifyOtpRegister.Email}_PendingUser");
            _cache.Remove($"{verifyOtpRegister.Email}_Verified");


            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthenticationModelDto
            {
                Email = user.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                Message = "OTP verified successfully. Registration completed."
            };
        }
        public async Task<AuthenticationModelDto> VerifyOtp(VerifyOtpRequestModelDto request)
        {
            var response = new AuthenticationModelDto();
            // 1. Check OTP from cache
            if (!_cache.TryGetValue($"{request.Email}_Verified", out OtpData storedOtpData))
            {
                response.IsAuthenticated = false;
                response.Message = "OTP expired or not found";
                return response;
            }

            // Check if OTP matches and has remaining checks
            if (storedOtpData.Code != request.Otp || storedOtpData.RemainingChecks <= 0 || storedOtpData.Expiry < DateTime.UtcNow)
            {
                response.IsAuthenticated = false;
                response.Message = "Invalid OTP or expired";
                return response;
            }

            // 2. Check user exists
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.IsAuthenticated = false;
                response.Message = "User not found";
                return response;
            }

            // Decrement remaining checks and update cache
            storedOtpData.RemainingChecks--;
            _cache.Set($"{request.Email}_Verified", storedOtpData, TimeSpan.FromMinutes(10));

            // 3. Set verification flag
            _cache.Set($"{request.Email}_OTP_Verified", true, TimeSpan.FromMinutes(10));

            response.IsAuthenticated = true;
            response.Message = "OTP verified successfully";
            response.Username = user.UserName;
            response.Email = user.Email;

            return response;

        }
        public async Task<AuthenticationModelDto> LoginAsync(LoginModelDto model)
        {
            var authenticationModel = new AuthenticationModelDto();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null ||!await _userManager.CheckPasswordAsync(user , model.Password))
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Email or password incorrect";
                return authenticationModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            authenticationModel.IsAuthenticated = true;
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationModel.ExpireOn = jwtSecurityToken.ValidTo;
            authenticationModel.Username = user.UserName;
            authenticationModel.Email = user.Email;
            authenticationModel.Roles = roles.ToList();
            authenticationModel.Message = "Login successful";
            return authenticationModel;
        }
        public async Task<AuthenticationModelDto> ResetPasswordWithOtp(ResetPasswordRequestModelDto request)
        {
            var response = new AuthenticationModelDto();

            // 1. Check if OTP was previously verified
            if (!_cache.TryGetValue($"{request.Email}_OTP_Verified", out bool isVerified) || !isVerified)
            {
                response.IsAuthenticated = false;
                response.Message = "OTP verification required first";
                return response;
            }

            // 2. Revalidate OTP
            if (!_cache.TryGetValue($"{request.Email}_Verified", out OtpData storedOtpData)
                || storedOtpData.Code != request.Otp
                || storedOtpData.RemainingChecks <= 0
                || storedOtpData.Expiry < DateTime.UtcNow)
            {
                response.IsAuthenticated = false;
                response.Message = "Invalid OTP or expired";
                return response;
            }

            // 3. Proceed with password reset
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.IsAuthenticated = false;
                response.Message = "User not found";
                return response;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);

            if(!result.Succeeded)
            {
                response.IsAuthenticated = false;
                response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            // 4. Clear OTP data after successful reset
            _cache.Remove($"{request.Email}_Verified");
            _cache.Remove($"{request.Email}_OTP_Verified");

            // 5. Generate JWT (if needed)
            var token = await CreateJwtToken(user);
            response.Token = new JwtSecurityTokenHandler().WriteToken(token);
            response.ExpireOn = token.ValidTo;
            response.IsAuthenticated = true;
            response.Message = "Password reset successfully";
            response.Username = user.UserName;
            response.Email = user.Email;

            return response;

        }

        public async Task<AuthenticationModelDto> SendOtpForPasswordReset(SendOtpRequestModelDto sendOtp)
        {
            AuthenticationModelDto authenticationModel = new AuthenticationModelDto();

            var user = await _userManager.FindByEmailAsync(sendOtp.Email);
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "User Not found";
                return authenticationModel;
            }

            var otp = await _otp.GenerateOtpAsync(sendOtp.Email);
            var emailResponse = await _email.SendEmailAsync(user.Email, "Your Password Reset OTP Code", $"Your OTP code for resetting your password is: {otp}");
            if (!emailResponse.IsAuthenticated)
            {
                return emailResponse;
            }

            authenticationModel.Message = "OTP sent successfully.";
            authenticationModel.IsAuthenticated = emailResponse.IsAuthenticated;
            return authenticationModel;

        }

        public async Task<string> AddToRole(AddRoleDto model)
        {
            var existUser = await _userManager.FindByNameAsync(model.UserName);
            if (existUser is null || !await _RoleManager.RoleExistsAsync(model.RoleName))
            {
                return "Invalid User or RoleName!";
            }
            if (await _userManager.IsInRoleAsync(existUser, model.RoleName))
            {
                return "User is assign to this role";
            }

            var result = await _userManager.AddToRoleAsync(existUser, model.RoleName);

            return result.Succeeded ? string.Empty : "Something went Wrong";
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken
                (
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInDays),
                signingCredentials: signingCredentials
                );
            return jwt;
        }
    }
}
