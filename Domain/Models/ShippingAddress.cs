
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class ShippingAddress : BaseEntity<string>
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; }
        public string PostalCode { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

    }
}
