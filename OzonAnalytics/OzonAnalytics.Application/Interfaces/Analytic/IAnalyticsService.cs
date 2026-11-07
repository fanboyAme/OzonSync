using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;

namespace OzonAnalytics.Application.Interfaces.Analytic
{
    public interface IAnalyticsService
    {
        public Task<SummaryResponseDto> GetSummaryAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct);
    }
}
