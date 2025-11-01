
namespace Domain.Models
{
    public class Category : BaseEntity<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
