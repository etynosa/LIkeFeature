using LIkeFeature.Models;

namespace LIkeFeature.Interfaces.IRepository
{
    public interface IArticleLikeRepository
    {
        Task<long> GetLikeCountAsync(string articleId);
        Task<bool> HasUserLikedAsync(string articleId, string userId);
        Task AddLikeAsync(ArticleLike like);
    }
}
