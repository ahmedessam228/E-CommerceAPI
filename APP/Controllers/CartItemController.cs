using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Shared.DTOs;
using Shared.DTOs.CartItems;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        public CartItemController(ICartItemsService cartItemsService)
        {
            _cartItemsService = cartItemsService;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCartItems()
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var items = await _cartItemsService.GetCartItemsByUserIdAsync(userId);

                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart items retrieved successfully",
                    data = items
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
        public async Task<IActionResult> AddCartItems([FromQuery] RequestCartItemsDto request)
        {
            try
            {
                var userId = User.FindFirst("uid").Value;

                var item = await _cartItemsService.AddCartItemAsync(userId, request);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart items retrieved successfully",
                    data = item
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

        [HttpPut("{cartitemId}")]
        public async  Task<IActionResult> UpdateCartItems([FromQuery] RequestCartItemsDto request , string cartitemId)
        {
            try
            {
                var userId = User.FindFirst("uid").Value;
                var item = await _cartItemsService.UpdateCartItemAsync(userId, cartitemId, request);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Cart Update successfully",
                    data = item
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{cartitemId}")]
        public async Task<IActionResult> DeleteCartItem(string cartitemId)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var message = await _cartItemsService.DeleteCartItemAsync(userId, cartitemId);

                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = message
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
