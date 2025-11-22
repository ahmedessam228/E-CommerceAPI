using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Shared.DTOs;
using Shared.DTOs.Cart;
using Shared.DTOs.Category;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var Carts = await _cartService.GetAllCarts(pageNumber, pageSize);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Carts retrieved successfully",
                    data = Carts
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
        [HttpGet("userId")]
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                var userId = User.FindFirst("uid").Value;
                var cart = await _cartService.GetCartByUserId(userId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart retrieved successfully",
                    data = cart
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
        [HttpGet("id/{cartId}")]
        public async Task<IActionResult> GetById(string cartid)
        {
            try
            {
                var cart = await _cartService.GetCartById(cartid);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart retrieved successfully",
                    data = cart
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
        public async Task<IActionResult> AddCart([FromQuery] RequestCartDto request)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var cart = await _cartService.AddCart(userId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart created successfully",
                    data = cart
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
        [HttpDelete]
        public async Task<IActionResult> DeleteCart([FromQuery] string cartId)
        {
            try
            {
                var result = await _cartService.DeleteCart(cartId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart deleted successfully",
                    data = result
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
