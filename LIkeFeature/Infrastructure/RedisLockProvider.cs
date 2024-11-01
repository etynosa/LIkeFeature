using LIkeFeature.Interfaces;
using StackExchange.Redis;

namespace LIkeFeature.Infrastructure
{
    public class RedisLockProvider : IDistributedLockProvider
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisLockProvider(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<IDistributedLockHandle> AcquireLockAsync(string key, TimeSpan timeout)
        {
            var db = _redis.GetDatabase();
            var value = Guid.NewGuid().ToString();
            var acquired = await db.StringSetAsync(
                $"lock:{key}",
                value,
                timeout,
                When.NotExists
            );

            return new RedisLockHandle(db, key, value, acquired);
        }

        private class RedisLockHandle : IDistributedLockHandle
        {
            private readonly IDatabase _db;
            private readonly string _key;
            private readonly string _value;
            private bool _disposed;

            public bool IsAcquired { get; }

            public RedisLockHandle(IDatabase db, string key, string value, bool acquired)
            {
                _db = db;
                _key = key;
                _value = value;
                IsAcquired = acquired;
            }

            public void Dispose()
            {
                if (_disposed) return;
                if (IsAcquired)
                {
                    // Only remove if it's still our lock
                    var script = @"
                    if redis.call('get', KEYS[1]) == ARGV[1] then
                        return redis.call('del', KEYS[1])
                    else
                        return 0
                    end";
                    _db.ScriptEvaluate(script, new RedisKey[] { $"lock:{_key}" }, new RedisValue[] { _value });
                }
                _disposed = true;
            }
        }
    }
}
