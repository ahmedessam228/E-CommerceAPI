
namespace Shared.DTOs.Reviews
{
    public class ResponseReviewsWithoutProductIdDto
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
    }
}
