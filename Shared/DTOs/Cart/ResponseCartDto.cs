
using Shared.DTOs.CartItems;

namespace Shared.DTOs.Cart
{
    public class ResponseCartDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public  ICollection<ResponseCartItemWithoutCartIdDto> CartItems { get; set; }
    }
}
