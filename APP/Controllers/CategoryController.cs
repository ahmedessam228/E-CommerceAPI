using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Category;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterCategoryDto filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try 
            {
                var categories = await _categoryService.GetAllCategories(filter,pageNumber , pageSize);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Categories retrieved successfully",
                    data = categories
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }
        [HttpGet("name/{Name}")]
        public async Task<IActionResult> GetByName(string Name)
        {
            try
            {
                var category = await _categoryService.GetCategoryByName(Name);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Category retrieved successfully",
                    data = category
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Category retrieved successfully",
                    data = category
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] RequestAddCategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }
            try 
            {
                var category = await _categoryService.AddCategory(request);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Category added successfully",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] RequestAddCategoryDto request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try 
            {
                var category = await _categoryService.UpdateCategory(id, request);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Category updated successfully",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try 
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }
    }
}
