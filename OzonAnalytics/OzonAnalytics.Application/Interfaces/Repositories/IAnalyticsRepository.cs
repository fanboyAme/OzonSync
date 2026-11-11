using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;
using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IAnalyticsRepository
    {
        public Task<SummaryResponseDto> GetFinancialSummaryAsync(DateTime from, DateTime to, Guid shopId, CancellationToken ct);
        public Task<List<TopProductDto>> GetTopProductsAsync(Guid shopId, DateTime from, DateTime to, int limit, TopProductSortBy sortBy, CancellationToken ct);
        public Task<List<TimeSeriesDto>> GetTimeSeriesAsync(Guid shopId, DateTime from, DateTime to, CancellationToken ct);
    }
}
