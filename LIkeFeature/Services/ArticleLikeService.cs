using LIkeFeature.Exceptions;
using LIkeFeature.Interfaces;
using LIkeFeature.Interfaces.IRepository;
using LIkeFeature.Models;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Data;

namespace LIkeFeature.Services
{
    public class ArticleLikeService : IArticleLikeService
    {
        //private readonly IConnectionMultiplexer _redis;
        private readonly IMemoryCache _cache;
        private readonly IArticleLikeRepository _repository;
        private readonly ILogger<ArticleLikeService> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        //private readonly IDistributedLockProvider _lockProvider;

        public ArticleLikeService(
            //IConnectionMultiplexer redis,
            IArticleLikeRepository repository,
            ILogger<ArticleLikeService> logger,
            IMemoryCache cache
            //IDistributedLockProvider lockProvider
            )
        {
            //_redis = redis;
            _repository = repository;
            _logger = logger;
            _cache = cache;
            //_lockProvider = lockProvider;
        }

        public async Task<long> GetLikeCountAsync(string articleId)
        {
            var cacheKey = $"article:likes:{articleId}";
            //var db = _redis.GetDatabase();

            //var cachedCount = await db.StringGetAsync(cacheKey);
            //if (cachedCount.HasValue)
            //{
            //    return (long)cachedCount;
            //}

            if (_cache.TryGetValue<long>(cacheKey, out var cachedCount))
            {
                return cachedCount;
            }

            var count = await _repository.GetLikeCountAsync(articleId);
            //await db.StringSetAsync(cacheKey, count, TimeSpan.FromMinutes(5));

            var cacheOptions = new MemoryCacheEntryOptions()
          .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
          .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(cacheKey, count, cacheOptions);
            return count;
        }

        //public async Task AddLikeAsync(string articleId, string userId)
        //{
        //    // Use distributed lock to prevent race conditions
        //    using var lockResult = await _lockProvider.AcquireLockAsync(
        //        $"like:lock:{articleId}:{userId}",
        //        TimeSpan.FromSeconds(5)
        //    );

        //    if (!lockResult.IsAcquired)
        //    {
        //        throw new ConcurrencyException("Could not acquire lock");
        //    }

        //    // Check if already liked
        //    if (await _repository.HasUserLikedAsync(articleId, userId))
        //    {
        //        throw new DuplicateLikeException();
        //    }

        //    await _repository.AddLikeAsync(new ArticleLike
        //    {
        //        ArticleId = articleId,
        //        UserId = userId,
        //        CreatedAt = DateTime.UtcNow
        //    });

        //    var db = _redis.GetDatabase();
        //    var cacheKey = $"article:likes:{articleId}";
        //    await db.StringIncrementAsync(cacheKey);
        //}

        public async Task AddLikeAsync(string articleId, string userId)
        {
            var cacheKey = $"article:likes:{articleId}";
            var userLikeKey = $"user:like:{articleId}:{userId}";

            if (_cache.TryGetValue<bool>(userLikeKey, out _))
            {
                throw new DuplicateLikeException();
            }

            try
            {
                await _semaphore.WaitAsync();

                if (await _repository.HasUserLikedAsync(articleId, userId))
                {
                    throw new DuplicateLikeException();
                }

                await _repository.AddLikeAsync(new ArticleLike
                {
                    ArticleId = articleId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });

                // Update cache
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(userLikeKey, true, cacheOptions);

                // Increment like count in cache if it exists
                if (_cache.TryGetValue<long>(cacheKey, out var currentCount))
                {
                    _cache.Set(cacheKey, currentCount + 1, cacheOptions);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> HasUserLikedAsync(string articleId, string userId)
        {
            var userLikeKey = $"user:like:{articleId}:{userId}";

            // Check cache first
            if (_cache.TryGetValue<bool>(userLikeKey, out var hasLiked))
            {
                return hasLiked;
            }

            // Check database
            hasLiked = await _repository.HasUserLikedAsync(articleId, userId);

            if (hasLiked)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _cache.Set(userLikeKey, true, cacheOptions);
            }

            return hasLiked;
        }

    }
}
