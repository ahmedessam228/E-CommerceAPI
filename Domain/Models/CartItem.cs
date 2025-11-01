using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class CartItem : BaseEntity<string>
    {
        public double Quantity { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        public string ProductId { get; set; }
        [JsonIgnore]
        public virtual Cart Cart { get; set; }
        public string CartId { get; set; }

    }
}
