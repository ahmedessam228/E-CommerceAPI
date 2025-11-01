
using Domain.Interfaces.Service;
using Microsoft.Extensions.Options;
using Shared.DTOs.Authentication;
using Shared.Setting;
using System.Net;
using System.Net.Mail;

namespace Service
{
   public class EmailService : IEmailService
   {
        private readonly SmtpSettings _smtpSettings;
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;

            Console.WriteLine($"Host: {_smtpSettings.Host}");
            Console.WriteLine($"Port: {_smtpSettings.Port}");
            Console.WriteLine($"Username: {_smtpSettings.UserName}");
            Console.WriteLine($"Password: {_smtpSettings.Password}");
        }
        public async Task<AuthenticationModelDto> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            AuthenticationModelDto response = new AuthenticationModelDto();
            try 
            {
                var Message = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.UserName, "E-commerce"),
                    To = { new MailAddress(recipientEmail) },
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);
                    client.EnableSsl = true;
                    await client.SendMailAsync(Message);
                }
                response.Message = "Email sent successfully.";
                response.IsAuthenticated = true;
            }
            catch (Exception ex)
            {
                response.Message = $"Failed to send email. Error: {ex.Message}";
                response.IsAuthenticated = false;
            }

            return response;

        }
   }
}
