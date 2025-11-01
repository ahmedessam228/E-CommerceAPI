
using Microsoft.AspNetCore.Http;

namespace Shared.DTOs.Product
{
    public class RequestProductDto
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double? StockQuantatiy { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? CategoryId { get; set; }

        //public  ICollection<RequestAddCartItemWithoutProductId> CartItems { get; set; } 
        //public  ICollection<RequestAddOrderItemWithoutProductId> OrderItems { get; set; }
        //public  ICollection<RequestAddReviewsWithoutProductId> Reviews { get; set; }
    }
}
