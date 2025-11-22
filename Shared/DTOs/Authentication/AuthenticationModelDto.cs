
using System.Text.Json.Serialization;

namespace Shared.DTOs.Authentication
{
    public class AuthenticationModelDto
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireOn { get; set; }
        public string? ProfilePicture { get; set; } = string.Empty;
    }
}
