using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Shared.DTOs.Order;
using Shared.DTOs.OrderItems;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(string userId)
        {
            var cart = await _unitOfWork.Repository<Cart, string>().GetFirstOrDefaultAsync(ui => ui.UserId == userId);
            if (cart == null)
                throw new Exception("Cart not found for this user.");

            var cartItems = await _unitOfWork.Repository<CartItem, string>()
                .GetAllAsync(ci => ci.CartId == cart.Id, includes: ci => ci.Product);

            if (cartItems == null || !cartItems.Any())
                throw new Exception("Cart is empty.");

            if (cartItems.Any(ci => ci.Product == null))
                throw new Exception("Some cart items have no linked product.");

            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending"
            };

            await _unitOfWork.Repository<Order, string>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };

                await _unitOfWork.Repository<OrderItem, string>().AddAsync(orderItem);
            }

            await _unitOfWork.SaveChangesAsync();
            foreach (var item in cartItems)
            {
                var cItem = await _unitOfWork.Repository<CartItem, string>().GetByIdAsync(item.Id);
                if (cItem != null)
                    _unitOfWork.Repository<CartItem, string>().RemoveAsync(cItem);
            }
            return new OrderResponseDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                Items = cartItems.Select(i => new OrderItemDto
                {
                    ProductId = i.Product.Id,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price
                }).ToList()
            };
        }
        public async Task<OrderResponseDto> GetOrderByIdAsync(string orderId)
        {
            var order = await _unitOfWork.Repository<Order, string>().GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            var orderItems = await _unitOfWork.Repository<OrderItem, string>()
                .GetAllAsync(oi => oi.Order.Id == orderId);

            return new OrderResponseDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                Items = orderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
        public async Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.Repository<Order, string>()
                            .GetAllAsync(o => o.UserId == userId);

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderDate = o.OrderDate,
            });
        }

        public async Task<OrderResponseDto> UpdateOrderAsync(string orderId, string newStatus)
        {
            var orderRepo = _unitOfWork.Repository<Order, string>();
            var order = await orderRepo.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found.");

            order.Status = newStatus;
            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return new OrderResponseDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
            };
        }
        public async Task<bool> DeleteOrderAsync(string orderId)
        {
            var orderRepo = _unitOfWork.Repository<Order, string>();
            var order = await orderRepo.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found.");

            var orderItemsRepo = _unitOfWork.Repository<OrderItem, string>();
            var orderItems = await orderItemsRepo.GetAllAsync(oi => oi.Order.Id == orderId);

            foreach (var item in orderItems)
                 orderItemsRepo.RemoveAsync(item);

            orderRepo.RemoveAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}
