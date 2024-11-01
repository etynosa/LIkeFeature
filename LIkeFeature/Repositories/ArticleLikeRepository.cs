using LIkeFeature.Data;
using LIkeFeature.Interfaces.IRepository;
using LIkeFeature.Models;
using Microsoft.EntityFrameworkCore;

namespace LIkeFeature.Repositories
{
    public class ArticleLikeRepository : IArticleLikeRepository
    {
        private readonly ArticleDbContext _context;

        public ArticleLikeRepository(ArticleDbContext context)
        {
            _context = context;
        }

        public async Task<long> GetLikeCountAsync(string articleId)
        {
            return await _context.ArticleLikes
                .Where(l => l.ArticleId == articleId)
                .CountAsync();
        }

        public async Task<bool> HasUserLikedAsync(string articleId, string userId)
        {
            return await _context.ArticleLikes
                .AnyAsync(l => l.ArticleId == articleId && l.UserId == userId);
        }

        public async Task AddLikeAsync(ArticleLike like)
        {
            await _context.ArticleLikes.AddAsync(like);
            await _context.SaveChangesAsync();
        }
    }

}
