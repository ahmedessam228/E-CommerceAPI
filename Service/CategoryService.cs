using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Service.MappingHelper;
using Shared.DTOs.Category;
using Shared.DTOs.Product;
using Shared.Helpers;
using System.Linq.Expressions;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ResponseCategoryDto>> GetAllCategories(FilterCategoryDto filter,int pageNumber, int PageSize)
        {
            Expression<Func<Category, bool>> predicate = c =>
                   (string.IsNullOrEmpty(filter.Name) || c.Name.Contains(filter.Name)) &&
                   (string.IsNullOrEmpty(filter.Description) || c.Description.Contains(filter.Description)) &&
                   (!filter.CreatedAt.HasValue || c.CreatedAt.Date == filter.CreatedAt.Value.Date);

            var categories = await _unitOfWork.Repository<Category, string>().GetAllAsync(predicate : predicate ,pageNumber: pageNumber , pageSize: PageSize, includes: c => c.Products );
            return MappingCategory.responseCategories(categories);
        }
        public async Task<ResponseCategoryDto> GetCategoryById(string id)
        {
            var category = await _unitOfWork.Repository<Category, string>().GetFirstOrDefaultAsync(
                predicate: c => c.Id == id,
                includes: c => c.Products
            );
            if (category == null)
                throw new Exception("Category with the same Id not exists.");
            return MappingCategory.responseCategory(category);
        }
        public async Task<ResponseCategoryDto> GetCategoryByName(string name)
        {
            var category = await _unitOfWork.Repository<Category, string>().GetFirstOrDefaultAsync(
                predicate: c => c.Name == name,
                includes: c => c.Products
            );
            if (category == null)
                throw new Exception("Category with the same Name not exists.");
            return MappingCategory.responseCategory(category);
        }
        public async Task<Category> AddCategory(RequestAddCategoryDto categoryDto)
        {
            var existingCategory = await _unitOfWork.Repository<Category, string>().GetFirstOrDefaultAsync(q => q.Name == categoryDto.Name);
            if (existingCategory != null)
            {
                throw new Exception("Category with the same Name already exists.");
            }

            var newCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                CreatedAt = categoryDto.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = categoryDto.UpdatedAt
            };

            await  _unitOfWork.Repository<Category, string>().AddAsync(newCategory);
            await  _unitOfWork.SaveChangesAsync();

            if (categoryDto.Products != null && categoryDto.Products.Any())
            {
                foreach (var p in categoryDto.Products)
                {
                    var newProduct = new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt,
                        StockQuantatiy = p.StockQuantatiy,
                        ImageUrl = p.ImageUrl != null ? $"{BaseUrl.BaseUrlValue}/Product/images/{p.ImageUrl.FileName}" : null,
                        CategoryId = newCategory.Id
                    };
                    await SaveFilesUpdate(p);

                    await _unitOfWork.Repository<Product, string>().AddAsync(newProduct);
                }
                await _unitOfWork.SaveChangesAsync();
            }
            return newCategory;
        }
        public async Task<Category?> UpdateCategory(string id, RequestAddCategoryDto categoryDto)
        {
            var existingCategory = await _unitOfWork.Repository<Category, string>()
                .FindBy(c => c.Id == id);

            if (existingCategory == null)
                throw new Exception("Category with the specified Id does not exist.");

            if (!string.IsNullOrEmpty(categoryDto.Name))
                existingCategory.Name = categoryDto.Name;

            if (!string.IsNullOrEmpty(categoryDto.Description))
                existingCategory.Description = categoryDto.Description;

            if (categoryDto.CreatedAt.HasValue)
                existingCategory.CreatedAt = categoryDto.CreatedAt.Value;

            existingCategory.UpdatedAt = categoryDto.UpdatedAt;

            _unitOfWork.Repository<Category, string>().Update(existingCategory);
            await _unitOfWork.SaveChangesAsync();

            if (categoryDto.Products != null && categoryDto.Products.Any())
            {
                foreach (var p in categoryDto.Products)
                {
                    var existingProduct = await _unitOfWork.Repository<Product, string>()
                        .GetFirstOrDefaultAsync(x => x.Name == p.Name);

                    if (existingProduct != null)
                    {
                        existingProduct.Name = p.Name ?? existingProduct.Name;
                        existingProduct.Description = p.Description ?? existingProduct.Description;
                        existingProduct.Price = p.Price != 0 ? p.Price : existingProduct.Price;
                        existingProduct.UpdatedAt = DateTime.UtcNow;
                        existingProduct.StockQuantatiy = p.StockQuantatiy != 0 ? p.StockQuantatiy : existingProduct.StockQuantatiy;
                        existingProduct.ImageUrl = p.ImageUrl != null
                            ? $"{BaseUrl.BaseUrlValue}/Product/images/{p.ImageUrl.FileName}"
                            : existingProduct.ImageUrl;

                        _unitOfWork.Repository<Product, string>().Update(existingProduct);
                    }
                    else
                    {
                        var newProduct = new Product
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            CreatedAt = p.CreatedAt ,
                            UpdatedAt = p.UpdatedAt ,
                            StockQuantatiy = p.StockQuantatiy,
                            ImageUrl = p.ImageUrl != null
                                ? $"{BaseUrl.BaseUrlValue}/Product/images/{p.ImageUrl.FileName}"
                                : null,
                            CategoryId = existingCategory.Id
                        };

                        await _unitOfWork.Repository<Product, string>().AddAsync(newProduct);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return existingCategory;
        }

        public async Task<string> DeleteCategoryAsync(string Categoryid)
        {
            var category = await _unitOfWork.Repository<Category, string>().GetByIdAsync(Categoryid);
            if (category == null)
                throw new Exception("Category with the same Id not exists.");
            _unitOfWork.Repository<Category, string>().RemoveAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return "Category deleted successfully.";
        }
        private async Task<string?> SaveFilesUpdate(RequestAddProductWithoutCatIDDto request)
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
