
using Shared.DTOs.CartItems;
using Shared.DTOs.OrderItems;
using Shared.DTOs.Reviews;

namespace Shared.DTOs.Product
{
    public class ResponseProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double StockQuantatiy { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryId { get; set; }

        //public  ICollection<ResponseCartItemWithoutProductIdDto> CartItems { get; set; } 
        //public  ICollection<ResponseOrderItemWithoutProductIdDto> OrderItems { get; set; }
        public  ICollection<ResponseReviewsWithoutProductIdDto> Reviews { get; set; }
    }
}
