namespace Shared.DTOs.Payment
{
    public class CreateOrUpdatePaymentDto
    {
        public string OrderId { get; set; }
        public string? PaymentId { get; set; } // = null if is a new payment process
        //public string PaymentMethod { get; set; } = "card";
        //public string Currency { get; set; } = "usd";
    }
}
