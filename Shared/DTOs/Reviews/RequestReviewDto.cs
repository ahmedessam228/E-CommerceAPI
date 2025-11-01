
namespace Shared.DTOs.Reviews
{
    public class RequestReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ProductId { get; set; }
    }
}
