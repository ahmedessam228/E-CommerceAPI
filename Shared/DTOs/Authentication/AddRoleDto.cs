
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication
{
    public class AddRoleDto
    {
        [Required]
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}
