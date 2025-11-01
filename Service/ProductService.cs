using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Service.MappingHelper;
using Shared.DTOs.Product;
using Shared.Helpers;
using System.Linq.Expressions;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ResponseProductDto>> GetAllProducts(FilterProductDto filter, int pageNumber, int pageSize)
        {
            Expression<Func<Product, bool>> predicate = p =>
                 (string.IsNullOrEmpty(filter.Name) || p.Name.Contains(filter.Name)) &&
                 (!filter.Price.HasValue || p.Price == filter.Price.Value) &&
                 (string.IsNullOrEmpty(filter.Description) || p.Description.Contains(filter.Description)) &&
                 (!filter.CreatedAt.HasValue || p.CreatedAt.Date == filter.CreatedAt.Value.Date) &&
                 (!filter.StockQuantatiy.HasValue || p.StockQuantatiy == filter.StockQuantatiy.Value) &&
                 (string.IsNullOrEmpty(filter.CategoryName) || p.Category.Name.Contains(filter.CategoryName));

            Expression<Func<Product, object>>[] includes =
            {
                p => p.CartItems,
                p => p.OrderItems,
                p => p.Reviews
            };

            var products = await _unitOfWork.Repository<Product, string>()
                .GetAllAsync
                ( 
                    predicate :predicate, 
                    pageNumber : pageNumber , 
                    pageSize : pageSize,
                    includes: includes
                );
            return MappingProduct.responseProducts(products);
        }

        public async Task<ResponseProductDto> GetProductById(string id)
        {
            Expression<Func<Product, object>>[] includes =
{
                p => p.CartItems,
                p => p.OrderItems,
                p => p.Reviews
            };
            var product = await _unitOfWork.Repository<Product, string>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == id,
                includes: includes
                );
            if (product == null)
                throw new Exception("Product with the same Id not exists.");
            return MappingProduct.responseProduct(product);
        }

        public async Task<IEnumerable<ResponseProductDto>> GetProductByNameOfCategory(string name)
        {
            Expression<Func<Product, object>>[] includes =
{
                p => p.CartItems,
                p => p.OrderItems,
                p => p.Reviews,
                p => p.Category
            };
            var product = await _unitOfWork.Repository<Product, string>().GetAllAsync(
                            predicate: p => p.Category.Name == name,
                            includes: includes
                            );
            if (product == null)
                throw new Exception("Product with the same Name not exists.");
            return MappingProduct.responseProducts(product);
        }

        public async Task<Product> AddProduct(RequestProductDto product)
        {
            await SaveFilesUpdate(product);
            var existingProduct = await _unitOfWork.Repository<Product, string>().FindBy(p => p.Name == product.Name);
            if (existingProduct != null)
            {
                throw new Exception("Product with the same Name already exists.");
            }
            var newProduct = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = product.UpdatedAt,
                StockQuantatiy = (double)product.StockQuantatiy,
                ImageUrl = product.ImageUrl != null ? $"{BaseUrl.BaseUrlValue}/Product/images/{product.ImageUrl.FileName}" : null,
                CategoryId = product.CategoryId,
                //CartItems = new List<CartItem>(),
                //OrderItems = new List<OrderItem>(),
                //Reviews = new List<Reviews>()
            };
            await _unitOfWork.Repository<Product, string>().AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();
            return newProduct;
        }
        public async Task<Product?> UpdateProduct(string id, RequestProductDto product)
        {
            var existingProductTask = _unitOfWork.Repository<Product, string>().GetByIdAsync(id);
            if (existingProductTask == null)
            {
                throw new Exception("Product with the same Id not exists.");
            }

            var existingProduct = existingProductTask.Result;
            if (existingProduct.Name != null) 
                existingProduct.Name = product.Name;
            if (existingProduct.Description != null)
                existingProduct.Description = product.Description;
            if (product.Price != null)
                existingProduct.Price = product.Price;
            if (product.CreatedAt != null)
                existingProduct.CreatedAt = (DateTime)product.CreatedAt;
            existingProduct.UpdatedAt = product.UpdatedAt;
            if (product.StockQuantatiy != null)
                existingProduct.StockQuantatiy = (double)product.StockQuantatiy;
            if (product.CategoryId != null)
                existingProduct.CategoryId = product.CategoryId;

            var imagePath = await SaveFilesUpdate(product);
            if (imagePath != null)
            {
                existingProduct.ImageUrl = $"{BaseUrl.BaseUrlValue}/Product/images/{product.ImageUrl.FileName}";
            }
            _unitOfWork.Repository<Product, string>().Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<string> DeleteProductAsync(string id)
        {
            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product with the same Id not exists.");
            }
            _unitOfWork.Repository<Product, string>().RemoveAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return "Product deleted successfully.";
        }

        private async Task<string?> SaveFilesUpdate(RequestProductDto request)
        {
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Product", "Images");
            Directory.CreateDirectory(imageFolder);

            var imageFile = request.ImageUrl;

            if (imageFile != null && imageFile.Length > 0)
            {
                var filePath = Path.Combine(imageFolder, imageFile.FileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(fileStream);
                return filePath;
            }
            return null;
        }
    }
}
