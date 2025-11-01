
namespace Domain.Models
{
    public class Order : BaseEntity<string>
    {
        public double TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
