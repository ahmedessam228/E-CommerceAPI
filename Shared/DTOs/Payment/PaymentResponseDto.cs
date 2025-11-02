namespace Shared.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public string PaymentId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string OrderId { get; set; } 
        public string paymentIntentID { get; set; }
    }

}
