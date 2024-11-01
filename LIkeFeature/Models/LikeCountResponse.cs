namespace LIkeFeature.Models
{
    public class LikeCountResponse
    {
        public long Count { get; set; }
    }

    public class LikeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
