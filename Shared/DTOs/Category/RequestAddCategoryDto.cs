using Shared.DTOs.Product;

namespace Shared.DTOs.Category
{
    public class RequestAddCategoryDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<RequestAddProductWithoutCatIDDto> Products { get; set; }
    }
}
