using LIkeFeature.Exceptions;
using LIkeFeature.Helpers;
using LIkeFeature.Interfaces;
using LIkeFeature.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace LIkeFeature.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleLikeController : ControllerBase
    {
        private readonly IArticleLikeService _likeService;
        private readonly ILogger<ArticleLikeController> _logger;

        public ArticleLikeController(IArticleLikeService likeService, ILogger<ArticleLikeController> logger)
        {
            _likeService = likeService;
            _logger = logger;
        }

        [HttpGet("{articleId}/count")]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "articleId" })]
        public async Task<ActionResult<LikeCountResponse>> GetLikeCount(string articleId)
        {
            try
            {
                var count = await _likeService.GetLikeCountAsync(articleId);
                return Ok(new LikeCountResponse { Count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting like count for article {ArticleId}", articleId);
                return StatusCode(500);
            }
        }

        [HttpPost("{articleId}/like")]
        [RateLimit(Name = "LikeLimit", Seconds = 3600, Limit = 10)]
        public async Task<ActionResult> LikeArticle(string articleId)
        {
            try
            {
                var userId = GetUserIdentifier();
                await _likeService.AddLikeAsync(articleId, userId);
                return Ok();
            }
            catch (DuplicateLikeException)
            {
                return BadRequest("Already liked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking article {ArticleId}", articleId);
                return StatusCode(500);
            }
        }

        [HttpGet("{articleId}/hasLiked")]
        public async Task<ActionResult<bool>> HasUserLiked(string articleId)
        {
            try
            {
                var userId = GetUserIdentifier();
                var hasLiked = await _likeService.HasUserLikedAsync(articleId, userId);
                return Ok(hasLiked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking like status for article {ArticleId}", articleId);
                return StatusCode(500);
            }
        }

        private string GetUserIdentifier()
        {
            // In production, this would come from authentication
            // For demo, we'll use IP + User Agent hash
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            return HashHelper.ComputeSHA256($"{ip}:{userAgent}");
        }
    }
}
