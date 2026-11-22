namespace OzonAnalytics.Application.ProjectDtos.SyncDTO
{
    public record class SyncDto(Guid shopId, DateTime? since, int? limit);
}
