using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Service.MappingHelper;
using Shared.DTOs.Reviews;
using System.Linq.Expressions;

namespace Service
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReviewService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;   
            _userManager = userManager;
        }
        public async Task<IEnumerable<ResponseReviewDto>> GetAllReviews(FilterReviewDto filter, int pageNumber, int pageSize)
        {
            Expression<Func<Reviews, bool>> predicate = r =>
                        (filter.ProductId == null || r.ProductId == filter.ProductId) &&
                        (filter.UserId == null || r.UserId == filter.UserId) &&
                        (!filter.Rating.HasValue || r.Rating == filter.Rating);

            var reviews = await _unitOfWork.Repository<Reviews, string>().GetAllAsync(
                predicate: predicate,
                pageNumber: pageNumber,
                pageSize: pageSize
                );
           
            return MappingReviews.responseReviews(reviews);
        }
        public async Task<ResponseReviewDto> GetReviewById(string id)
        {
            var review = await _unitOfWork.Repository<Reviews, string>().GetByIdAsync(id);
            if (review == null)
                throw new Exception("Review with this Id not found");
            return MappingReviews.responseReview(review);
        }

        public async Task<Reviews> AddReview(string userId, RequestReviewDto review)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(review.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            var newReview = new Reviews
            {
                Id = Guid.NewGuid().ToString(),
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                ProductId = review.ProductId,
                UserId = userId,
            };
            await _unitOfWork.Repository<Reviews , string>().AddAsync(newReview);
            await _unitOfWork.SaveChangesAsync();
            return newReview;
        }
        public async Task<Reviews?> UpdateReview(string id,string userId ,RequestReviewDto review)
        {
            var existingReview = await _unitOfWork.Repository<Reviews, string>().GetByIdAsync(id);
            if (existingReview == null)
                throw new Exception("Review with this Id not found");

            if (existingReview.UserId!= userId)
                throw new UnauthorizedAccessException("You can only edit your own reviews.");

            if(existingReview.Rating != null)
                 existingReview.Rating = review.Rating;

            if (existingReview.Comment != null)
                existingReview.Comment = review.Comment;

            if (existingReview.ProductId != null)
                existingReview.ProductId = review.ProductId;

            _unitOfWork.Repository<Reviews, string>().Update(existingReview);
            await _unitOfWork.SaveChangesAsync();
            return existingReview;
        }

        public async Task<string> DeleteReviewAsync(string id , string userId)
        {
            var review = await _unitOfWork.Repository<Reviews, string>().GetByIdAsync(id);
            if(review == null)
                throw new Exception("Review with this Id not found");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            _unitOfWork.Repository<Reviews, string>().RemoveAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return "Review Delete Successfully";
        }
    }
}
