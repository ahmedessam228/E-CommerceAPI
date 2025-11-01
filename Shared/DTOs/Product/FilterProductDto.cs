
namespace Shared.DTOs.Product
{
    public class FilterProductDto
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public double? StockQuantatiy { get; set; }
        public string? CategoryName { get; set; }
    }
}
