namespace OzonAnalytics.Application.Interfaces.Sync
{
    public interface ISyncLock
    {
        public Task<Guid?> TryAcquireAsync(Guid shopId, TimeSpan ttl);
        public Task<bool> ReleaseAsync(Guid shopId, Guid token);
    }
}
