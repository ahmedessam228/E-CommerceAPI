
using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Service.MappingHelper;
using Shared.DTOs.Cart;

namespace Service
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartService(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IEnumerable<ResponseCartDto>> GetAllCarts(int pageNumber, int pageSize)
        {
            var Carts = await _unitOfWork.Repository<Cart , string>().GetAllAsync(pageNumber : pageNumber, pageSize : pageSize);
            return MappingCart.responseCarts(Carts);
        }
        public async Task<ResponseCartDto> GetCartById(string cartId)
        {
            var existingCart = await _unitOfWork.Repository<Cart, string>().GetByIdAsync(cartId);

            if (existingCart == null) 
                throw new Exception("Cart with this Id not found");
          
            var cart = await _unitOfWork.Repository<Cart , string>().GetFirstOrDefaultAsync(
            predicate: c => c.Id == cartId,
            includes: c => c.CartItems
            );
            return MappingCart.responseCart(cart);
        }

        public async Task<ResponseCartDto> GetCartByUserId(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
                throw new Exception("UserId Not exists");

            var cart = await  _unitOfWork.Repository<Cart , string>().GetFirstOrDefaultAsync(
                predicate: c => c.UserId == userId,
                includes: c => c.CartItems
                );
            return MappingCart.responseCart(cart);
        }

        public async Task<Cart> AddCart(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
                throw new Exception("UserId Not exists");

            var existingCart = await _unitOfWork.Repository<Cart, string>()
                .GetFirstOrDefaultAsync(c => c.UserId == userId);

            if (existingCart != null)
                throw new Exception("Cart already exists for this user.");

            var newCart = new Cart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Cart, string>().AddAsync(newCart);
            await _unitOfWork.SaveChangesAsync();

            return newCart;
        }
        public async Task<string> DeleteCart(string cartId)
        {
            var cart = await _unitOfWork.Repository<Cart , string>().GetByIdAsync(cartId);
            if (cart == null)
                throw new Exception("Cart with this Id not found");
            _unitOfWork.Repository<Cart, string>().RemoveAsync(cart);
            await _unitOfWork.SaveChangesAsync();
            return "Cart deleted successfully.";
        }
    }
}
