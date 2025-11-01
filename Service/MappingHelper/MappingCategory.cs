
using Domain.Models;
using Shared.DTOs.Category;
using Shared.DTOs.Product;

namespace Service.MappingHelper
{
    public static class MappingCategory
    {
        public static List<ResponseCategoryDto> responseCategories(IEnumerable<Category> categories)
        {
            return categories.Select(c => new ResponseCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Products = c.Products?.Select(p => new ResponseProductWithoutCategoryIdDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    StockQuantatiy = p.StockQuantatiy,
                    ImageUrl = p.ImageUrl
                }).ToList()
            }).ToList();    
        }

        public static ResponseCategoryDto responseCategory(Category c)
        {
            return new ResponseCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Products = c.Products?.Select(p => new ResponseProductWithoutCategoryIdDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    StockQuantatiy = p.StockQuantatiy,
                    ImageUrl = p.ImageUrl
                }).ToList()
            };
        }
    }
}
