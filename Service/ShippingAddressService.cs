using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Service.MappingHelper;
using Shared.DTOs.ShippingAddress;


namespace Service
{
    public class ShippingAddressService : IShippingAddress
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ShippingAddressService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IEnumerable<ResponseShippingAddressDto>> GetUserShippingAddressesAsync(string userId)
        {
            var addresses = await _unitOfWork.Repository<ShippingAddress, string>()
                .GetAllAsync(sa => sa.UserId == userId);
            return MappingShippingAddress.ShippingAddresses(addresses);
        }

        public async Task<ShippingAddress> AddShippingAddressAsync(string userId, RequestShippingAddressDto requestDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var existingAddresses = await _unitOfWork.Repository<ShippingAddress, string>()
                .GetFirstOrDefaultAsync(sa => sa.UserId == userId && 
                sa.Street == requestDto.Street &&
                sa.City == requestDto.City && 
                sa.Country == requestDto.Country 
                );
            if (existingAddresses != null)
            {
                throw new Exception("Shippung address is exist for you");
            }
            var newAddress = new ShippingAddress
            {
                Id = Guid.NewGuid().ToString(),
                Country = requestDto.Country,
                City = requestDto.City,
                Street = requestDto.Street,
                IsDefault = requestDto.IsDefault,
                PostalCode = requestDto.PostalCode,
                UserId = userId
            };
            await _unitOfWork.Repository<ShippingAddress , string>().AddAsync(newAddress);
            await _unitOfWork.SaveChangesAsync();
            return newAddress;
        }


        public async Task<ShippingAddress> UpdateShippingAddressAsync(string userId, string addressId, RequestShippingAddressDto requestDto)
        {
            var address = await _unitOfWork.Repository<ShippingAddress, string>()
                .GetFirstOrDefaultAsync(sa => sa.UserId == userId && sa.Id == addressId);
            if (address == null)
            {
                throw new Exception("Shipping address not found for this user");
            }

            if (requestDto.Country != null)
                address.Country = requestDto.Country;

            if (requestDto.City != null)
                address.City = requestDto.City;

            if (requestDto.Street != null)
                address.Street = requestDto.Street;

            if (requestDto.IsDefault != null)
                address.IsDefault = requestDto.IsDefault;

            if (requestDto.PostalCode != null)
                address.PostalCode = requestDto.PostalCode;
            _unitOfWork.Repository<ShippingAddress, string>().Update(address);
            await _unitOfWork.SaveChangesAsync();
            return address;
        }
        public async Task<string> DeleteShippingAddressAsync(string userId, string addressId)
        {
            var address = await _unitOfWork.Repository<ShippingAddress, string>()
                .GetFirstOrDefaultAsync(sa => sa.UserId == userId && sa.Id == addressId);
            if (address == null)
            {
                throw new Exception("Shipping address not found for this user");
            }
            _unitOfWork.Repository<ShippingAddress, string>().RemoveAsync(address);
            await _unitOfWork.SaveChangesAsync();
            return "Shipping address deleted successfully";
        }
    }
}
