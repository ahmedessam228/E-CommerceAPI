
using Domain.Models;
using Shared.DTOs.ShippingAddress;

namespace Service.MappingHelper
{
    public static class MappingShippingAddress
    {
        public static List<ResponseShippingAddressDto> ShippingAddresses(IEnumerable<ShippingAddress> addresses)
        {
            return addresses.Select(address => new ResponseShippingAddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                Country = address.Country,
                PostalCode = address.PostalCode,
                IsDefault = address.IsDefault
            }).ToList();
        }

        public static ResponseShippingAddressDto ShippingAddress(ShippingAddress address)
        {
            return new ResponseShippingAddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                Country = address.Country,
                PostalCode = address.PostalCode,
                IsDefault = address.IsDefault
            };
        }
    }
}
