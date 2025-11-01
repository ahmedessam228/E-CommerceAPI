using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Service;
using Shared.DTOs;
using Shared.DTOs.ShippingAddress;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddress _shippingAddressService;
        public ShippingAddressController(IShippingAddress shippingAddressService)
        {
            _shippingAddressService = shippingAddressService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserShippingAddresses()
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }
                var addresses = await _shippingAddressService.GetUserShippingAddressesAsync(userId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "User shipping addresses retrieved successfully",
                    data = addresses
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
        public async Task<IActionResult> AddShippingAddress([FromQuery] RequestShippingAddressDto requestDto)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }
                var newAddress = await _shippingAddressService.AddShippingAddressAsync(userId, requestDto);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Shipping address added successfully",
                    data = newAddress
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
        [HttpPut("{addressId}")]

        public async Task<IActionResult> UpdateShippingAddress(string addressId, [FromQuery] RequestShippingAddressDto requestDto)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var updatedAddress = await _shippingAddressService.UpdateShippingAddressAsync(userId, addressId, requestDto);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Shipping address updated successfully",
                    data = updatedAddress
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
        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteShippingAddress(string addressId)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var result = await _shippingAddressService.DeleteShippingAddressAsync(userId, addressId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Shipping address deleted successfully",
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
