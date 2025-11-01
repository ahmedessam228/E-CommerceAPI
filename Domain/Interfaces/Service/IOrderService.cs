using Shared.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Service
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(string userId);
        Task<OrderResponseDto> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<OrderResponseDto> UpdateOrderAsync(string orderId, string newStatus);
        Task<bool> DeleteOrderAsync(string orderId);
    }
}
