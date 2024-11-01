using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Memory;

namespace LIkeFeature.Models
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RateLimitAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
        public int Limit { get; set; }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            //var redis = context.HttpContext.RequestServices
            //    .GetRequiredService<IConnectionMultiplexer>();
            //var db = redis.GetDatabase();
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();


            var key = $"ratelimit:{Name}:{context.HttpContext.User.Identity.Name}";
            //var count = await db.StringIncrementAsync(key);
            //if (count == 1)
            //{
            //    await db.KeyExpireAsync(key, TimeSpan.FromSeconds(Seconds));
            //}

            //if (count > Limit)
            //{
            //    context.Result = new StatusCodeResult(429);
            //    return;
            //}
            if (!cache.TryGetValue(key, out int count))
            {
                // If not found, set the count to 1 and set the expiration time
                count = 1;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(Seconds));

                cache.Set(key, count, cacheEntryOptions);
            }
            else
            {
                // Increment the count if found
                count++;
                cache.Set(key, count);
            }

            // Check if the limit has been exceeded
            if (count > Limit)
            {
                context.Result = new StatusCodeResult(429); // Too Many Requests
                return;
            }



            await next();
        }
    }
}
