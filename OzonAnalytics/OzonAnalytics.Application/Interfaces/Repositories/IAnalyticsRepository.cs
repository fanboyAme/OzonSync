using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IAnalyticsRepository
    {
        public Task<SummaryResponseDto> GetFinancialSummaryAsync(DateTime from, DateTime to, Guid shopId, CancellationToken ct);
    }
}
