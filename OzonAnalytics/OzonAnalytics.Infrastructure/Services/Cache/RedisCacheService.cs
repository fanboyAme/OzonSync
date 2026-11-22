using OzonAnalytics.Application.Interfaces.Cache;
using StackExchange.Redis;

namespace OzonAnalytics.Infrastructure.Services.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        public RedisCacheService(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
        }
        public async Task<string?> GetValueByKey(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        public async Task<bool> SetValueAsync(string key, string value, TimeSpan ttl)
        {
            return await _db.StringSetAsync(key, value, ttl);
        }
        public async Task<long> GetVersionAsync(Guid shopId)
        {
            var versionKey = AnalyticsCacheKeysService.Version(shopId);
            var value = await GetValueByKey(versionKey);

            if (value != null && long.TryParse(value, out long res))
            {
                return res;
            }
            return 0;
        }
        public async Task<long> IncreaseVersionCacheShop(Guid shopId)
        {
            return await _db.StringIncrementAsync(AnalyticsCacheKeysService.Version(shopId));
        }
        public string SetKey(Guid shopId, long version, string method, string parametr) => AnalyticsCacheKeysService.Data(shopId, version, method, parametr);
    }
}
