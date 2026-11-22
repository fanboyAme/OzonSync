namespace OzonAnalytics.Application.ProjectDtos.SyncDTO
{
    public record class GetLogsDto(Guid shopId, int? limit);
}
