using Shared.DTOs.Authentication;

namespace Domain.Interfaces.Service
{
    public interface IEmailService
    {
        Task<AuthenticationModelDto> SendEmailAsync(string recipientEmail, string subject, string body);
    }
}
