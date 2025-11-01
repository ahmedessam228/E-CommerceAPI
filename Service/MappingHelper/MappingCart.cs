
using Domain.Models;
using Shared.DTOs.Cart;
using Shared.DTOs.CartItems;
using Shared.DTOs.Product;

namespace Service.MappingHelper
{
    public static class MappingCart
    {
        public static List<ResponseCartDto> responseCarts(IEnumerable<Cart> carts)
        {
            return carts.Select(c => new ResponseCartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt,
                CartItems = c.CartItems?.Select(ci => new ResponseCartItemWithoutCartIdDto
                {
                    Id = ci.Id,
                    Quantity = ci.Quantity,
                    ProductId = ci.ProductId
                }).ToList()
            }).ToList();
        }
        public static ResponseCartDto responseCart(Cart c)
        {
            return new ResponseCartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt,
                CartItems = c.CartItems?.Select(ci => new ResponseCartItemWithoutCartIdDto
                {
                    Id = ci.Id,
                    Quantity = ci.Quantity,
                    ProductId = ci.ProductId
                }).ToList()
            };
        }
    }
}
