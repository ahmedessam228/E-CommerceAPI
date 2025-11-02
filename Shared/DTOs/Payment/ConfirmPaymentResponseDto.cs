using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Payment
{
    public class ConfirmPaymentResponseDto
    {
        public string PaymentId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string OrderId { get; set; }
    }
}
