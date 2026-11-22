using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface ISyncLogRepository
    {
        public Task AddAsync(SyncLog syncLog, CancellationToken ct);
        public Task UpdateAsync(SyncLog syncLog, CancellationToken ct);
        public Task<List<SyncLog>> GetLogsByShopIdAsync(Guid shopId, int? limit, CancellationToken ct);
    }
}
