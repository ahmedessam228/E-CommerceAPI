
using Domain.Models;
using Shared.DTOs.ShippingAddress;

namespace Domain.Interfaces.Service
{
    public interface IShippingAddress
    {
        Task<IEnumerable<ResponseShippingAddressDto>> GetUserShippingAddressesAsync(string userId);
        Task<ShippingAddress> AddShippingAddressAsync(string userId, RequestShippingAddressDto requestDto);
        Task<ShippingAddress> UpdateShippingAddressAsync(string userId, string addressId,RequestShippingAddressDto requestDto);
        Task<string> DeleteShippingAddressAsync(string userId, string addressId);
    }
}
