using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.OrderItems
{
    public class ResponseOrderItemDto
    {
        public string Id { get; set; } 
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
