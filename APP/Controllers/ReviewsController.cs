using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Reviews;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReviews([FromQuery] FilterReviewDto filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var reviews = await _reviewService.GetAllReviews(filter, pageNumber, pageSize);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Reviews fetched successfully",
                    data = reviews
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
        public async Task<IActionResult> GetReviewById([FromRoute] string id)
        {
            try
            {
                var review = await _reviewService.GetReviewById(id);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Review fetched successfully",
                    data = review
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
        public async Task<IActionResult> AddReview([FromQuery] RequestReviewDto request)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var review = await _reviewService.AddReview(userId, request);

                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Review add successfully",
                    data = review
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
        public async Task<IActionResult> UpdateReview(string id,[FromQuery] RequestReviewDto request)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var review = await _reviewService.UpdateReview(id, userId, request);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Review Updated successfully",
                    data = review
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
        public async Task<IActionResult> DelteReview(string id)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var reviwe = await _reviewService.DeleteReviewAsync(id , userId);
                return Ok(new GeneralResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Reviwe delete successfully",
                    data = reviwe
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
