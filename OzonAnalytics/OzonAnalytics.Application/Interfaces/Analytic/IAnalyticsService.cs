using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;
using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.Interfaces.Analytic
{
    public interface IAnalyticsService
    {
        public Task<SummaryResponseDto> GetSummaryAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct);
        public Task<CompareResponseDto> CompareAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct);
        public Task<List<TopProductDto>> TopProductAsync(Guid shopId, Guid userId, DateTime from, DateTime to, int? limit, TopProductSortBy sortBy, CancellationToken ct);
        public Task<List<TimeSeriesDto>> TimeSeriesAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct);
    }
}
