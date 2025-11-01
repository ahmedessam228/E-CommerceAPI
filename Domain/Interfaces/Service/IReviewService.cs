
using Domain.Models;
using Shared.DTOs.Reviews;

namespace Domain.Interfaces.Service
{
    public interface IReviewService
    {
        Task<IEnumerable<ResponseReviewDto>> GetAllReviews(FilterReviewDto filter, int pageNumber, int pageSize);
        Task<ResponseReviewDto> GetReviewById(string id);
        Task<Reviews> AddReview(string userId,RequestReviewDto review);
        Task<Reviews?> UpdateReview(string id,string userId ,RequestReviewDto review);
        Task<string> DeleteReviewAsync(string id , string userId);
    }
}
