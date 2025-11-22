using Azure;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Authentication;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        // Add your action methods here that utilize _authenticationService
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModelDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return Ok(new GeneralResponse { statusCode = StatusCodes.Status200OK, message = result.Message });

            return Ok(
                new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Registration initiated successfully. Please verify OTP sent to your email."
                });
        }
        [HttpPost("verify-otp-for-register")]
        public async Task<IActionResult> VerifyOtpForRegister([FromBody] VerifyOtpRequestModelDto verifyOtpRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authenticationService.SendOtpForRegister(verifyOtpRequest);

            if (!result.IsAuthenticated)
                return Ok(new GeneralResponse { statusCode = StatusCodes.Status200OK, message = result.Message });
            setRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpireOn); 
            return Ok(
                new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Registration completed successfully.",
                    data = new
                    {
                        token = result.Token,
                        expireDate = result.ExpireOn,
                        user = new
                        {
                            username = result.Username,
                            email = result.Email,
                        }
                    }
                }
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModelDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authenticationService.LoginAsync(model);
            if (!result.IsAuthenticated)
                return Ok(new GeneralResponse { statusCode = StatusCodes.Status200OK, message = result.Message });
            if(!string.IsNullOrEmpty(result.RefreshToken))
                setRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpireOn);
            return Ok(
                new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Login successful.",
                    data = new
                    {
                        token = result.Token,
                        expireDate = result.ExpireOn,
                        user = new
                        {
                            username = result.Username,
                            email = result.Email,
                        }
                    }
                }
            );

        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] SendOtpRequestModelDto sendOtp)
        {
            var response = await _authenticationService.SendOtpForPasswordReset(sendOtp);

            if (!response.IsAuthenticated)
            {
                return BadRequest(new GeneralResponse { statusCode = StatusCodes.Status400BadRequest, message = response.Message });
            }

            return Ok(new GeneralResponse { statusCode = StatusCodes.Status200OK, message = response.Message });
        }

        [HttpPost("verify-otp-for-reset-password")]
        public async Task<IActionResult> VerifyOtpForResetPassword([FromBody] VerifyOtpRequestModelDto verifyOtpRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authenticationService.VerifyOtp(verifyOtpRequest);

            if (!result.IsAuthenticated)
                return Ok(new GeneralResponse { statusCode = StatusCodes.Status200OK, message = result.Message });

            return Ok(
                new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "OTP verified successfully. You can now reset your password."
                }
            );
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> RestPassword([FromBody] ResetPasswordRequestModelDto resetPassword)
        {
            var result = await _authenticationService.ResetPasswordWithOtp(resetPassword);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new GeneralResponse { statusCode = StatusCodes.Status400BadRequest, message = result.Message });
            }
            setRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpireOn);

            return Ok(new GeneralResponse
            {
                statusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = new
                {
                    token = result.Token,
                    expireDate = result.ExpireOn,
                    user = new
                    {
                        username = result.Username,
                        email = result.Email,
                    }
                }
            });
        }

        [HttpPost("add-to-role")]
        public async Task<IActionResult> AddToRole([FromBody] AddRoleDto addRole)
        {
            var result = await _authenticationService.AddToRole(addRole);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = result,
                });
            }
            return Ok(new GeneralResponse
            {
                statusCode = StatusCodes.Status200OK,
                message = "User added To Role Successfully"
            }
            );

        }
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refresh = Request.Cookies["refreshToken"];

            var result = await _authenticationService.RefreshTokenAsync(refresh);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = result.Message,
                });
            }
            setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpireOn);

            return Ok(new GeneralResponse
            {
                statusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = new
                {
                    token = result.Token,
                    expireDate = result.ExpireOn,
                    user = new
                    {
                        username = result.Username,
                        email = result.Email,
                    }
                }
            });
        }
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto tokenRequest)
        {
            var token = tokenRequest.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Token is required"
                });
            }
            var result = await _authenticationService.RevokeToken(token);
            if (!result)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Token revocation failed"
                });
            }
            return Ok(new GeneralResponse
            {
                statusCode = StatusCodes.Status200OK,
                message = "Token revoked successfully"
            });
        }

        private void setRefreshTokenInCookie(string refreshToken , DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        [Authorize]
        [HttpGet("Hello")]
        public IActionResult GetData()
        {
            return Ok("Hello RefrshToken");
        }
    }
}
