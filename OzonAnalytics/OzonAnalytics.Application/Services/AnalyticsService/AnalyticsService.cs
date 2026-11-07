using Microsoft.Extensions.Logging;
using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Analytic;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;

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
        private async Task RequestChecker(DateTime from, DateTime to, Guid userId, Guid shopId, CancellationToken ct)
        {
            var existsShop = await _shopRepository.GetByIdAsync(shopId, ct) ?? throw new NotFoundException("Магазин не найден");

            if (existsShop.UserId != userId) throw new ForbiddenException("Отказано в доступе");

            if (from >= to || (to - from) > TimeSpan.FromDays(366)) throw new ValidationException("Ошибка входных данных");
        }
        public async Task<SummaryResponseDto> GetSummaryAsync(Guid shopId, Guid userId, DateTime from, DateTime to, CancellationToken ct)
        {
            //Даты считаются UTC
            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);
            await RequestChecker(fromUtc, toUtc, userId, shopId, ct);

            var finansialData = await _analyticsRepository.GetFinancialSummaryAsync(fromUtc, toUtc, shopId, ct);

            return finansialData;
        }
    }
}