
using Domain.Models;
using Shared.DTOs.CartItems;

namespace Domain.Interfaces.Service
{
    public interface ICartItemsService
    {
        Task<CartItem> AddCartItemAsync(string userId, RequestCartItemsDto dto);
        Task<CartItem> UpdateCartItemAsync(string userId, string cartItemId, RequestCartItemsDto dto);
        Task<string> DeleteCartItemAsync(string userId, string cartItemId);
        Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(string userId);
    }
}
