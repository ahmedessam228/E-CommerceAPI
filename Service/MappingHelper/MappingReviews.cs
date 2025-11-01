using Domain.Models;
using Shared.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingHelper
{
    public static class MappingReviews
    {
        public static IEnumerable<ResponseReviewDto> responseReviews(IEnumerable<Reviews> reviews)
        {
            var responseReviews = reviews.Select(r => new ResponseReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                ProductId = r.ProductId,
                UserId = r.UserId
            });
            return responseReviews;
        }
        public static ResponseReviewDto responseReview(Reviews review)
        {
            var responseReview = new ResponseReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                ProductId = review.ProductId,
                UserId = review.UserId
            };
            return responseReview;
        }
    }
}
