
namespace Domain.Interfaces.Service
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string email, bool isForRegistration = false);
        Task RemoveOtpAsync(string email);
    }
}
