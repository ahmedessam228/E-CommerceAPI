using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.OrderItems
{
    public class RequestOrderItemDto
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public double Quantity { get; set; }
    }
}
