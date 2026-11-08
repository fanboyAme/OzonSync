using Microsoft.Extensions.Logging;
using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Analytic;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;
using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.Services.AnalyticService
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ILogger<AnalyticsService> _logger;
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly IShopRepository _shopRepository;
        public AnalyticsService(IAnalyticsRepository analyticsRepository, ILogger<AnalyticsService> logger, IShopRepository shopRepository)
        {
            _analyticsRepository = analyticsRepository;
            _logger = logger;
            _shopRepository = shopRepository;
        }
        private static DateTime ToUtc(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        private async Task<DateTimeUtcDto> RequestChecker(DateTime from, DateTime to, Guid userId, Guid shopId, CancellationToken ct)
        {
            var fromUtc = ToUtc(from);
            var toUtc = ToUtc(to);

            var existsShop = await _shopRepository.GetByIdAsync(shopId, ct) ?? throw new NotFoundException("Магазин не найден");

            if (existsShop.UserId != userId) throw new ForbiddenException("Отказано в доступе");

            if (fromUtc >= toUtc || (toUtc - fromUtc) > TimeSpan.FromDays(366)) throw new ValidationException("Ошибка входных данных");

            return new DateTimeUtcDto(FromUtc: fromUtc, ToUtc: toUtc);

        }
        private decimal? CalcPercent(decimal diff, decimal previous)
        {
            if (previous == 0)
            {
                return null;
            }
            return Math.Round(diff / previous * 100, 2);
        }
        private async Task<SummaryResponseDto> GetSummaryInternalAsync(DateTime from, DateTime to, Guid shopId, CancellationToken ct)
        {
            return await _analyticsRepository.GetFinancialSummaryAsync(from, to, shopId, ct);
        }
        public async Task<SummaryResponseDto> GetSummaryAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct)
        {
            //Даты считаются UTC
            var dateList = await RequestChecker(from, to, userId, shopId, ct);

            return await GetSummaryInternalAsync(dateList.FromUtc, dateList.ToUtc, shopId, ct);
        }
        public async Task<CompareResponseDto> CompareAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct)
        {
            var dateList = await RequestChecker(from, to, userId, shopId, ct);

            var lenghtPeriod = dateList.ToUtc - dateList.FromUtc;

            var previousFrom = dateList.FromUtc - lenghtPeriod;

            var currentFinansialData = await GetSummaryInternalAsync(dateList.FromUtc, dateList.ToUtc, shopId, ct);
            var previousFinansialData = await GetSummaryInternalAsync(previousFrom, dateList.FromUtc, shopId, ct);

            var orderCountDiff = currentFinansialData.OrderCount - previousFinansialData.OrderCount;
            var revenueDiff = currentFinansialData.Revenue - previousFinansialData.Revenue;
            var payoutDiff = currentFinansialData.Payout - previousFinansialData.Payout;
            var orderCountDiffPercent = CalcPercent(orderCountDiff, previousFinansialData.OrderCount);
            var revenueDiffPercent = CalcPercent(revenueDiff, previousFinansialData.Revenue);
            var payoutDiffPercent = CalcPercent(payoutDiff, previousFinansialData.Payout);

            var deltaSummary = new DeltaSummary(OrderCountDiff: orderCountDiff, RevenueDiff: revenueDiff, PayoutDiff: payoutDiff, OrderCountDiffPercent: orderCountDiffPercent, RevenueDiffPercent: revenueDiffPercent, PayoutDiffPercent: payoutDiffPercent);

            return new CompareResponseDto(Current: currentFinansialData, Previous: previousFinansialData, Delta: deltaSummary);
        }
        public async Task<List<TopProductDto>> TopProductAsync(Guid shopId, Guid userId, DateTime from, DateTime to, int? limit, TopProductSortBy sortBy, CancellationToken ct)
        {
            var dateList = await RequestChecker(from, to, userId, shopId, ct);

            var take = Math.Clamp(limit ?? 10, 1, 50);

            return await _analyticsRepository.GetTopProductsAsync(shopId, dateList.FromUtc, dateList.ToUtc, take, sortBy, ct);
        }
    }
}
