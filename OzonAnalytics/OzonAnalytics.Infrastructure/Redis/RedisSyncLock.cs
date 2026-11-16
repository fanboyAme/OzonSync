using OzonAnalytics.Application.Interfaces.Sync;
using StackExchange.Redis;

namespace OzonAnalytics.Infrastructure.Redis
{
    public class RedisSyncLock : ISyncLock
    {
        private readonly IDatabase _db;

        public RedisSyncLock(IConnectionMultiplexer connectionMultiplexer)
        {
            _db = connectionMultiplexer.GetDatabase();
        }
        private const string ReleaseLuaScript = @"
        if redis.call('get', KEYS[1]) == ARGV[1] then
               return redis.call('del', KEYS[1])
            else
                return 0
        end";
        public async Task<Guid?> TryAcquireAsync(Guid shopId, TimeSpan ttl)
        {
            var token = Guid.NewGuid();
            var acquire = await _db.StringSetAsync($"sync-lock:{shopId}", token.ToString(), ttl, when: When.NotExists);

            return acquire ? token : null;
        }
        public async Task<bool> ReleaseAsync(Guid shopId, Guid token)
        {
            var result = await _db.ScriptEvaluateAsync(
                ReleaseLuaScript,
                keys: new RedisKey[] { $"sync-lock:{shopId}" },
                values: new RedisValue[] { token.ToString() }
                );
            return (int)result == 1;

        }
    }
}
