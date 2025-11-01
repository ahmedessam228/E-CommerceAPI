using Domain.Models;
using Shared.DTOs.Product;

namespace Domain.Interfaces.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ResponseProductDto>> GetAllProducts(FilterProductDto filter, int pageNumber, int PageSize);
        Task<ResponseProductDto> GetProductById(string id);
        Task<IEnumerable<ResponseProductDto>> GetProductByNameOfCategory(string name);
        Task<Product> AddProduct(RequestProductDto product);
        Task<Product?> UpdateProduct(string id, RequestProductDto product);
        Task<string> DeleteProductAsync(string id);
    }
}
