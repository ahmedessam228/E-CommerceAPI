
using Shared.DTOs.Authentication;

namespace Domain.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationModelDto> RegisterAsync(RegisterModelDto model);
        Task<AuthenticationModelDto> SendOtpForRegister(VerifyOtpRequestModelDto verifyOtpRequest);
        Task<AuthenticationModelDto> LoginAsync(LoginModelDto model);
        Task<AuthenticationModelDto> SendOtpForPasswordReset(SendOtpRequestModelDto sendOtp);
        Task<AuthenticationModelDto> VerifyOtp(VerifyOtpRequestModelDto verifyOtpRequest);
        Task<AuthenticationModelDto> ResetPasswordWithOtp(ResetPasswordRequestModelDto resetPasswordRequest);
        Task<string> AddToRole(AddRoleDto addRole);
    }
}
