
using Domain.Models;
using Shared.DTOs.Cart;

namespace Domain.Interfaces.Service
{
    public interface ICartService
    {
        Task<IEnumerable<ResponseCartDto>> GetAllCarts(int pageNumber, int PageSize);
        Task<ResponseCartDto> GetCartByUserId(string userId);
        Task<ResponseCartDto> GetCartById(string cartId);
        Task<Cart> AddCart(string userId);
        Task<string> DeleteCart(string cartId);

    }
}
