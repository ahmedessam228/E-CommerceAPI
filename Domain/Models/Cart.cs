
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Cart : BaseEntity<string>
    {
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        public  string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
