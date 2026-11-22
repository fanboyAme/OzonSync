namespace OzonAnalytics.Application.Interfaces.Cache
{
    public interface IAnalyticsCache
    {
        public Task<T> GetOrSetAsync<T>(Guid shopId, string method, string parametr, Func<Task<T>> factory);
    }
}
