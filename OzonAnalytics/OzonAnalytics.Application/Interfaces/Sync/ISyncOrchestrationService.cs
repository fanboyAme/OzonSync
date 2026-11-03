using OzonAnalytics.Application.ProjectDtos.SyncDTO;

namespace OzonAnalytics.Application.Interfaces.Orders
{
    public interface ISyncOrchestrationService
    {
        public Task SyncShopAsync(Guid shopId, Guid userId, DateTime? since, int? limit, CancellationToken ct);
        public Task<List<SyncLogDto>> GetLogsAsync(Guid shopId, int? limit, Guid userId, CancellationToken ct);
    }
}
