
namespace Shared.DTOs.ShippingAddress
{
    public class ResponseShippingAddressDto
    {
        public string Id { get; set; }  
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; }
        public string PostalCode { get; set; }
    }
}
