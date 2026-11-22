using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.ProjectDtos.SyncDTO
{
    public record class SyncLogDto(Guid Id, Guid ShopId, DateTime StartedAt, DateTime? FinishedAt, SyncStatus Status, int OrdersSynced, string? ErrorMessage);
}
