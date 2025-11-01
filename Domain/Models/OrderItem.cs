
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class OrderItem : BaseEntity<string>
    {
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        public string ProductId { get; set; }
        [JsonIgnore]

        public virtual Order Order { get; set; }
        public string OrderId { get; set; }
    }
}
