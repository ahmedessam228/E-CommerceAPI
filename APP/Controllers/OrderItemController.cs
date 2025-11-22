using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.OrderItems;
using System.Security.Claims;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class OrderItemController : ControllerBase
    {
        private IOrderItemService _orderItemService;
        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrderItem([FromQuery] RequestOrderItemDto dto)
        {
            try
            {
                var userId = User.FindFirst("uid").Value;
                var result = await _orderItemService.CreateOrderItemAsync(dto, userId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "OrderItem created",
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderItemById(string id)
        {
            try
            {
                var result = await _orderItemService.GetOrderItemByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = "Order item not found." });

                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "OrderItem created",
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(string id, [FromBody] RequestOrderItemDto dto)
        {
            try
            {
                var userId = User.FindFirst("uid").Value;
                var result = await _orderItemService.UpdateOrderItemAsync(id, dto, userId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "OrderItem created",
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(string id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var deleted = await _orderItemService.DeleteOrderItemAsync(id, userId);

                if (!deleted)
                    return NotFound(new { message = "Order item not found or not authorized to delete." });

                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Order item deleted successfully.",
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
