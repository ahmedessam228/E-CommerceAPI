using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class GeneralResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; } = string.Empty;
        public object? data { get; set; } = null;
        public ICollection<string>? errors { get; set; } = new List<string>();
    }
}
