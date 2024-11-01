
namespace LIkeFeature.Models
{
    public class ArticleLike
    {
        public int Id { get; set; }
        public string ArticleId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
