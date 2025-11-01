using Domain.Models;
using Shared.DTOs.OrderItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Service
{
    public interface IOrderItemService
    {
        Task<OrderItem> CreateOrderItemAsync(RequestOrderItemDto dto , string userId);
        Task<ResponseOrderItemDto> GetOrderItemByIdAsync(string id);
        Task<IEnumerable<ResponseOrderItemDto>> GetOrderItemsByOrderIdAsync(string orderId);
        Task<OrderItem> UpdateOrderItemAsync(string id, RequestOrderItemDto dto, string userId);
        Task<bool> DeleteOrderItemAsync(string id, string userId);
    }
}
