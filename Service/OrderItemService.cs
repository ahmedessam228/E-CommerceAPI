using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Shared.DTOs.OrderItems;


namespace Service
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<OrderItem> CreateOrderItemAsync(RequestOrderItemDto dto, string userId)
        {
            var order = await _unitOfWork.Repository<Order, string>().GetByIdAsync(dto.OrderId);
            if (order == null || order.UserId != userId)
                throw new Exception("Order not found or doesn't belong to the user.");


            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found.");

            var totalPrice = product.Price * dto.Quantity;

            var orderitem = new OrderItem 
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = totalPrice
            };
            await _unitOfWork.Repository<OrderItem , string>().AddAsync(orderitem);
            order.TotalAmount += totalPrice;
            _unitOfWork.Repository<Order, string>().Update(order);
            await _unitOfWork.SaveChangesAsync();
            return orderitem;
        }

        public async Task<bool> DeleteOrderItemAsync(string id, string userId)
        {
            var orderItemRepo = _unitOfWork.Repository<OrderItem, string>();
            var orderItem = await orderItemRepo.GetByIdAsync(id);
            if (orderItem == null)
                throw new Exception("OrderItem not found.");

            var order = await _unitOfWork.Repository<Order, string>().GetByIdAsync(orderItem.OrderId);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to delete this order item.");

            orderItemRepo.RemoveAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ResponseOrderItemDto> GetOrderItemByIdAsync(string id)
        {
            var item = await _unitOfWork.Repository<OrderItem, string>().GetByIdAsync(id);
            return new ResponseOrderItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
        }

        public async Task<IEnumerable<ResponseOrderItemDto>> GetOrderItemsByOrderIdAsync(string orderId)
        {
            var items = await _unitOfWork.Repository<OrderItem, string>().GetAllAsync(oi => oi.OrderId == orderId);

            return items.Select(i => new ResponseOrderItemDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            });
        }

        public async Task<OrderItem> UpdateOrderItemAsync(string id, RequestOrderItemDto dto, string userId)
        {
            var orderItem = await _unitOfWork.Repository<OrderItem, string>().GetByIdAsync(id);
            if (orderItem == null)
                throw new Exception("Order item not found.");

            var order = await _unitOfWork.Repository<Order, string>().GetByIdAsync(orderItem.OrderId);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.UserId != userId)
                throw new Exception("You are not authorized to modify this order.");

            var product = await _unitOfWork.Repository<Product, string>().GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found.");

            var oldTotal = orderItem.UnitPrice;
            var newTotal = product.Price * dto.Quantity;

            orderItem.ProductId = dto.ProductId;
            orderItem.Quantity = dto.Quantity;
            orderItem.UnitPrice = newTotal;

            _unitOfWork.Repository<OrderItem, string>().Update(orderItem);

            order.TotalAmount = order.TotalAmount - oldTotal + newTotal;
            _unitOfWork.Repository<Order, string>().Update(order);

            await _unitOfWork.SaveChangesAsync();

            return orderItem;
        }

    }
}
