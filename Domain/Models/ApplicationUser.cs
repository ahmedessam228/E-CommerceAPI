using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual Cart Cart { get; set; }
        public virtual ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
        [JsonIgnore]
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
