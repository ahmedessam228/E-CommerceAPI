
namespace Shared.DTOs.CartItems
{
    public class RequestCartItemsDto
    {
        public double Quantity { get; set; }
        public string ProductId { get; set; }
        public string CartId { get; set; }
    }
}
