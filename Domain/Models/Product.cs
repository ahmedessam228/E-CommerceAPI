
namespace Domain.Models
{
    public class Product : BaseEntity<string>
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public double StockQuantatiy { get; set; }
        public string ImageUrl { get; set; }

        public virtual Category Category{ get; set; }
        public string CategoryId { get; set; }
        public virtual List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual List<Reviews> Reviews { get; set; } = new List<Reviews>();
    }
}
