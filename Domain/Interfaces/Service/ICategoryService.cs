using Domain.Models;
using Shared.DTOs.Category;
namespace Domain.Interfaces.Service
{
    public interface ICategoryService
    {
        Task<IEnumerable<ResponseCategoryDto>> GetAllCategories(FilterCategoryDto filter,int pageNumber , int PageSize);
        Task<ResponseCategoryDto> GetCategoryById(string id);
        Task<ResponseCategoryDto> GetCategoryByName(string name);
        Task<Category> AddCategory(RequestAddCategoryDto category);
        Task<Category?> UpdateCategory(string id, RequestAddCategoryDto category);
        Task<string> DeleteCategoryAsync(string id);
    }
}
