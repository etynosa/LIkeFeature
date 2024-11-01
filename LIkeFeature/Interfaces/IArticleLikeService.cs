namespace LIkeFeature.Interfaces
{
    public interface IArticleLikeService
    {
        Task<long> GetLikeCountAsync(string articleId);
        Task AddLikeAsync(string articleId, string userId);
        Task<bool> HasUserLikedAsync(string articleId, string userId);
    }
}
