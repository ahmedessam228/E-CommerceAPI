
namespace Shared.DTOs.OrderItems
{
    public class ResponseOrderItemWithoutProductIdDto
    {
        public string Id { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string OrderId { get; set; }
    }
}
