using Shared.DTOs.Payment;

namespace Domain.Interfaces.Service
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CraeteOrUpdatePaymentAsync(CreateOrUpdatePaymentDto dto);
        Task<ConfirmPaymentResponseDto> ConfirmPaymentAsync(string paymentIntentId);

        //Task<PaymentResponseDto> CreateAndConfirmTestPaymentAsync(string orderId);
    }
}
