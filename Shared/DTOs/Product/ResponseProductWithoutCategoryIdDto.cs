
namespace Shared.DTOs.Product
{
    public class ResponseProductWithoutCategoryIdDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double StockQuantatiy { get; set; }
        public string ImageUrl { get; set; }

    }
}
