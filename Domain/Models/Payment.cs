
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Payment : BaseEntity<string>
    {
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
