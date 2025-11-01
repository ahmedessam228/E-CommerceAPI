
using Shared.DTOs.Product;

namespace Shared.DTOs.Category
{
    public class ResponseCategoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<ResponseProductWithoutCategoryIdDto> Products { get; set; }
    }
}
