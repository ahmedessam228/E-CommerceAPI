
namespace Shared.DTOs.ShippingAddress
{
    public class RequestShippingAddressDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; }
        public string PostalCode { get; set; }
    }
}
