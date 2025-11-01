
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Reviews : BaseEntity<string>
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        public string ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
