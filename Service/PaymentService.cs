using Domain.Interfaces;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Shared.DTOs.Payment;
using Stripe;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }
        public async Task<PaymentResponseDto> CraeteOrUpdatePaymentAsync(CreateOrUpdatePaymentDto dto)
        {
            var order = await _unitOfWork.Repository<Order, string>().GetByIdAsync(dto.OrderId);
            if (order == null)
                throw new Exception("Order not found.");

            var amount = order.TotalAmount;

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (!string.IsNullOrEmpty(dto.PaymentId))
            {
                var existingPayment = await _unitOfWork.Repository<Payment, string>().GetByIdAsync(dto.PaymentId);
                if (existingPayment == null)
                    throw new Exception("Payment not found.");

                paymentIntent = await paymentIntentService.UpdateAsync(existingPayment.TransactionId, new PaymentIntentUpdateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "usd",
                });
                //Console.WriteLine(paymentIntent.Id);

                existingPayment.Amount = amount;
                existingPayment.Status = paymentIntent.Status;
                existingPayment.PaymentDate = DateTime.UtcNow;
                

                await _unitOfWork.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    PaymentId = existingPayment.Id,
                    Amount = existingPayment.Amount,
                    Status = existingPayment.Status,
                    TransactionId = existingPayment.TransactionId,
                    PaymentDate = existingPayment.PaymentDate,
                    OrderId = existingPayment.OrderId,
                    paymentIntentID = paymentIntent.Id
                };
            }
            else
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await paymentIntentService.CreateAsync(option);
               // Console.WriteLine(paymentIntent.Id);

                var payment = new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = "card",
                    Status = paymentIntent.Status,
                    TransactionId = paymentIntent.Id,
                    Order = order
                };

                await _unitOfWork.Repository<Payment, string>().AddAsync(payment);

                order.Status = "Pending Payment";

                await _unitOfWork.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    PaymentId = payment.Id,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    TransactionId = payment.TransactionId,
                    PaymentDate = payment.PaymentDate,
                    OrderId = order.Id,
                    paymentIntentID = paymentIntent.Id
                };
            }
        }

        public async Task<ConfirmPaymentResponseDto> ConfirmPaymentAsync(string paymentIntentId)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
                throw new ArgumentException("PaymentIntentId is required", nameof(paymentIntentId));

            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = await paymentIntentService.ConfirmAsync(paymentIntentId, new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa" 
            });

            var payment = await _unitOfWork.Repository<Payment, string>()
                .GetFirstOrDefaultAsync(
                    p => p.TransactionId == paymentIntentId,
                    includes: p => p.Order
                );

            if (payment == null)
                throw new Exception("Payment not found.");

            payment.Status = paymentIntent.Status;

            if (payment.Order != null)
            {
                payment.Order.Status = paymentIntent.Status == "succeeded" ? "Paid" : "Payment Failed";
            }

            await _unitOfWork.SaveChangesAsync();

            return new ConfirmPaymentResponseDto
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate,
                OrderId = payment.OrderId,
            };
        }


        #region testAndConfirm
        /*
        public async Task<PaymentResponseDto> CreateAndConfirmTestPaymentAsync(string orderId)
        {
            var order = await _unitOfWork.Repository<Order, string>().GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            var amount = order.TotalAmount;

            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" } 
            });

            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "pm_card_visa", 
                Status = paymentIntent.Status,
                TransactionId = paymentIntent.Id,
                OrderId = order.Id,
            };

            await _unitOfWork.Repository<Payment, string>().AddAsync(payment);

            var confirmOptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa"
            };

            paymentIntent = await paymentIntentService.ConfirmAsync(paymentIntent.Id, confirmOptions);

            payment.Status = paymentIntent.Status;
            order.Status = paymentIntent.Status == "succeeded" ? "Paid" : "Payment Failed";

            await _unitOfWork.SaveChangesAsync();

            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate
            };
        }
        */
        #endregion

    }
}
