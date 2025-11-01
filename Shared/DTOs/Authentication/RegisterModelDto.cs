
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication
{
    public class RegisterModelDto
    {
        [Required]
        public string Username { get; set; }

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string Password { get; set; }

        [Required, StringLength(150)]
        [Compare("Password", ErrorMessage = "Password confirm do not match password.")]
        public string ConfirmPassword { get; set; }
    }
}
