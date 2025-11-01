
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication
{
    public class SendOtpRequestModelDto
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
