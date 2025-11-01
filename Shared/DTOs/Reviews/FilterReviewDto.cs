
namespace Shared.DTOs.Reviews
{
    public class FilterReviewDto
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserId { get; set; }
        public string? ProductId { get; set; }
    }
}
