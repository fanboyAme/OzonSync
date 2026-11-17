namespace OzonAnalytics.Application.Interfaces.Cache
{
    public interface ICacheService
    {
        public Task<string?> GetValueByKey(string key);
        public Task<bool> SetValueAsync(string key, string value, TimeSpan ttl);
        public Task<long> GetVersionAsync(Guid shopId);
        public Task<long> IncreaseVersionCacheShop(Guid shopId);
        public string SetKey(Guid shopId, long version, string method, string parametr);
    }
}
