using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.CartItems;

namespace Service
{
    public class CartItemService :ICartItemsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartItemService(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<CartItem> AddCartItemAsync(string userId, RequestCartItemsDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
                throw new Exception("User not found");

            var cart = await _unitOfWork.Repository<Cart, string>().GetFirstOrDefaultAsync(c => c.UserId == userId);
            if(cart==null)
                throw new Exception("Cart not found for the user");

            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(dto.ProductId);
            if(product==null)
                throw new Exception("Product not found");

            if (dto.Quantity > product.StockQuantatiy)
                throw new Exception($"Not enough stock available. Only {product.StockQuantatiy} items left.");

            //check if item in cartItem
            var existingItem = await _unitOfWork.Repository<CartItem, string>()
                    .GetFirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == product.Id);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + dto.Quantity;
                if (newQuantity > product.StockQuantatiy)
                    throw new Exception($"Not enough stock available. Only {product.StockQuantatiy} items left.");

                existingItem.Quantity = newQuantity;
                product.StockQuantatiy -= dto.Quantity;
                await _unitOfWork.SaveChangesAsync();
                return existingItem;
            }


            var cartItem = new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                Quantity = dto.Quantity,
                ProductId = dto.ProductId,
                CartId = cart.Id
            };
            await _unitOfWork.Repository<CartItem, string>().AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return cartItem;
        }

        public async Task<string> DeleteCartItemAsync(string userId, string cartItemId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
                throw new Exception("User not found");

            var cartItem = await _unitOfWork.Repository<CartItem, string>().GetByIdAsync(cartItemId);
            if(cartItem==null)
                throw new Exception("Cart item not found");

            var cart = await _unitOfWork.Repository<Cart, string>().GetByIdAsync(cartItem.CartId);
            if(cart.UserId != userId)
                throw new Exception("Unauthorized access to cart item");

            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(cartItem.ProductId);
            if (product != null)
            {
                product.StockQuantatiy += cartItem.Quantity;
                _unitOfWork.Repository<Product, string>().Update(product);
            }

            _unitOfWork.Repository<CartItem, string>().RemoveAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return "item Delete Successfully";
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(string userId)
        {
            var cart = await _unitOfWork.Repository<Cart, string>().GetFirstOrDefaultAsync(c => c.UserId == userId);
            if(cart==null)
                throw new Exception("Cart not found for the user");

            var cartItems = await _unitOfWork.Repository<CartItem, string>().GetAllAsync(
                ci => ci.CartId == cart.Id ,
                includes : q=>q.Product
                );
            return cartItems;

        }

        public async Task<CartItem> UpdateCartItemAsync(string userId, string cartItemId, RequestCartItemsDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
                throw new Exception("User not found");

            var cartItem = await _unitOfWork.Repository<CartItem, string>().GetByIdAsync(cartItemId);
            if(cartItem==null)
                throw new Exception("Cart item not found");

            var cart = await _unitOfWork.Repository<Cart, string>().GetByIdAsync(cartItem.CartId);
            if(cart.UserId != userId)
                throw new Exception("Unauthorized access to cart item");

            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            double quantityDifference = dto.Quantity - cartItem.Quantity; 

            if (quantityDifference > 0)//3
            {
                if (quantityDifference > product.StockQuantatiy)
                    throw new Exception($"Not enough stock available. Only {product.StockQuantatiy} items left.");

                product.StockQuantatiy -= quantityDifference;
            }
            else if (quantityDifference < 0) 
            {
                product.StockQuantatiy += quantityDifference;
            }

            if (cartItem.ProductId != null)
                cartItem.ProductId = dto.ProductId;

            if (cartItem.Quantity != null)
                cartItem.Quantity = dto.Quantity;
            _unitOfWork.Repository<CartItem, string>().Update(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return cartItem;
        }
    }
}
