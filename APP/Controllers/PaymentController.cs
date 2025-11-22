using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Payment;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdatePayment([FromQuery] CreateOrUpdatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _paymentService.CraeteOrUpdatePaymentAsync(dto);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "process success",
                    data = response
                });
            }
            catch (Stripe.StripeException ex)
            {
                return BadRequest(new GeneralResponse {
                    statusCode = StatusCodes.Status502BadGateway,
                    message = $"Stripe error: {ex.Message}" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = $"Stripe error: {ex.Message}"
                });
            }
        }


        [HttpPost("confirm/{paymentIntentId}")]
        public async Task<IActionResult> ConfirmPayment(string paymentIntentId)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
                return BadRequest("Invalid Payment Intent ID");

            try
            {
                var response = await _paymentService.ConfirmPaymentAsync(paymentIntentId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "process success",
                    data = response
                });
            }
            catch (Stripe.StripeException ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status502BadGateway,
                    message = $"Stripe error: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = $"Stripe error: {ex.Message}"
                });
            }
        }

        #region ConfirmAndTest
        /*

        [HttpPost("create-and-confirm-test/{orderId}")]
        public async Task<IActionResult> CreateAndConfirmTestPayment(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return BadRequest(new { message = "Invalid OrderId" });

            try
            {
                var response = await _paymentService.CreateAndConfirmTestPaymentAsync(orderId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Test payment confirmed successfully",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = ex.Message
                });
            }
        }
        */
        #endregion

    }
}
