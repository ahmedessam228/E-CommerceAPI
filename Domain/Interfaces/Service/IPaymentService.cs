using Shared.DTOs.Payment;

namespace Domain.Interfaces.Service
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CraeteOrUpdatePaymentAsync(CreateOrUpdatePaymentDto dto);
        Task<PaymentResponseDto> ConfirmPaymentAsync(string paymentIntentId);
        Task<PaymentResponseDto> CreateAndConfirmTestPaymentAsync(string orderId);

    }
}
