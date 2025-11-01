using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication
{
    public class VerifyOtpRequestModelDto
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}
