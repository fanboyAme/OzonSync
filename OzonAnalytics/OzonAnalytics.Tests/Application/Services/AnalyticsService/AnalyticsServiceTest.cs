using Microsoft.Extensions.Logging;
using Moq;
using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Cache;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.Services.AnalyticService
{
    public class AnalyticsServiceTest
    {
        private readonly Mock<ILogger<AnalyticsService>> _logger = new();
        private readonly Mock<IAnalyticsRepository> _analyticsRepository = new();
        private readonly Mock<IShopRepository> _shopRepository = new();
        private readonly Mock<IAnalyticsCache> _analyticsCache = new();

        private AnalyticsService CreateService() => new(_analyticsRepository.Object, _logger.Object, _shopRepository.Object, _analyticsCache.Object);
        private static DateTime ToUtc(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        [Fact]
        public async Task GetSummaryAsync_ForeignShop_ThrowsForbidden()
        {
            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            var data = CreateService();

            await Assert.ThrowsAsync<ForbiddenException>(() => data.GetSummaryAsync(shopId, otherUserId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, CancellationToken.None));
        }
        [Fact]
        public async Task GetSummaryAsync_InvalidPeriod_MoreOneYear_ThrowsValidation()
        {
            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            var dateTimeFromIntervalMoreOneYear = DateTime.UtcNow.AddYears(-3);
            var dateTimeToIntervalMoreOneYear = DateTime.UtcNow.AddYears(-1);

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            var data = CreateService();

            await Assert.ThrowsAsync<ValidationException>(() => data.GetSummaryAsync(shopId, ownerUserId, dateTimeFromIntervalMoreOneYear, dateTimeToIntervalMoreOneYear, CancellationToken.None));
        }
        [Fact]
        public async Task GetSummaryAsync_InvalidPeriod_FromHighTo_ThrowsValidation()
        {
            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            var dateTimeFromHighTo = DateTime.UtcNow.AddYears(-1);
            var dateTimeToLowFrom = DateTime.UtcNow.AddYears(-2);

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            var data = CreateService();

            await Assert.ThrowsAsync<ValidationException>(() => data.GetSummaryAsync(shopId, ownerUserId, dateTimeFromHighTo, dateTimeToLowFrom, CancellationToken.None));
        }
        [Fact]
        public async Task GetSummary_SuccessReturnData()
        {
            var statusCountList = new List<StatusCountDto> { new StatusCountDto(OrderStatusAndSubStatus.Assembling, 2), new StatusCountDto(OrderStatusAndSubStatus.Cancelled, 19) };
            var response = new SummaryResponseDto(10, 100, 200m, 10m, 23m, 10m, statusCountList);
            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            var dateTimeFrom = DateTime.UtcNow.AddDays(-7);
            var dateTimeTo = DateTime.UtcNow;

            var data = CreateService();

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            _analyticsCache.Setup(ac => ac.GetOrSetAsync<SummaryResponseDto>(shopId, "summary", $"{dateTimeFrom:O}:{dateTimeTo:O}", It.IsAny<Func<Task<SummaryResponseDto>>>())).Returns<Guid, string, string, Func<Task<SummaryResponseDto>>>((shopid, method, parametr, factory) => factory());

            _analyticsRepository.Setup(ar => ar.GetFinancialSummaryAsync(dateTimeFrom, dateTimeTo, shopId, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            var result = await data.GetSummaryAsync(shopId, ownerUserId, dateTimeFrom, dateTimeTo, CancellationToken.None);

            Assert.Equal(response, result);
        }
        [Fact]
        public async Task Compare_SuccessReturnData()
        {
            var statusCountList = new List<StatusCountDto> { new StatusCountDto(OrderStatusAndSubStatus.Assembling, 2), new StatusCountDto(OrderStatusAndSubStatus.Cancelled, 19) };
            var currentFinansialData = new SummaryResponseDto(12, 100, 200m, 10m, 23m, 10m, statusCountList);
            var previousFinansialData = new SummaryResponseDto(10, 200, 300m, 20m, 33m, 20m, statusCountList);
            var deltaSummary = new DeltaSummary(
                currentFinansialData.OrderCount - previousFinansialData.OrderCount,
                currentFinansialData.Revenue - previousFinansialData.Revenue,
                currentFinansialData.Payout - previousFinansialData.Payout,
                Math.Round((currentFinansialData.OrderCount - previousFinansialData.OrderCount) / (decimal)previousFinansialData.OrderCount * 100, 2),
                Math.Round((currentFinansialData.Revenue - previousFinansialData.Revenue) / (decimal)previousFinansialData.Revenue * 100, 2),
                Math.Round((currentFinansialData.Payout - previousFinansialData.Payout) / (decimal)previousFinansialData.Payout * 100, 2));

            var compareResponse = new CompareResponseDto(currentFinansialData, previousFinansialData, deltaSummary);

            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            var dateTimeFrom = DateTime.UtcNow.AddDays(-7);
            var dateTimeTo = DateTime.UtcNow;

            var lenghtPeriod = dateTimeTo - dateTimeFrom;

            var previousFrom = dateTimeFrom - lenghtPeriod;

            var data = CreateService();

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            _analyticsCache.Setup(ac => ac.GetOrSetAsync<SummaryResponseDto>(shopId, "summary", $"{dateTimeFrom:O}:{dateTimeTo:O}", It.IsAny<Func<Task<SummaryResponseDto>>>())).ReturnsAsync(currentFinansialData);
            _analyticsCache.Setup(ac => ac.GetOrSetAsync<SummaryResponseDto>(shopId, "summary", $"{previousFrom:O}:{dateTimeFrom:O}", It.IsAny<Func<Task<SummaryResponseDto>>>())).ReturnsAsync(previousFinansialData);

            var result = await data.CompareAsync(shopId, ownerUserId, dateTimeFrom, dateTimeTo, CancellationToken.None);

            Assert.Equal(compareResponse, result);
        }
        [Fact]
        public async Task TopProductAsync_ValidRequest_ReturnsTopProducts()
        {


            var ProductId = Guid.NewGuid();
            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            var dateTimeFrom = DateTime.UtcNow.AddDays(-7);
            var dateTimeTo = DateTime.UtcNow;
            var response = new List<TopProductDto> { new TopProductDto(ProductId, 1, "name", 2, 2, 2) };
            var data = CreateService();

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            _analyticsCache.Setup(ac => ac.GetOrSetAsync<List<TopProductDto>>(shopId, "top-product", $"{dateTimeFrom:O}:{dateTimeTo:O}:{10}:{TopProductSortBy.Items}", It.IsAny<Func<Task<List<TopProductDto>>>>())).ReturnsAsync(response);

            var result = await data.TopProductAsync(shopId, ownerUserId, dateTimeFrom, dateTimeTo, 10, TopProductSortBy.Items, CancellationToken.None);

            Assert.Equal(response, result);
        }
        [Fact]
        public async Task TimeSeriesAsync_MissingDays_FillsWithZeros()
        {
            var shopId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            var shop = new Shop(ownerUserId, "client", "key", "name", DateTime.UtcNow);

            var dateTimeFrom = DateTime.UtcNow.AddDays(-4);
            var dateTimeTo = dateTimeFrom.AddDays(+4);
            var DateList = new List<TimeSeriesDto> { new TimeSeriesDto(dateTimeFrom, 1, 2, 2), new TimeSeriesDto(dateTimeFrom.AddDays(1), 1, 2, 2), new TimeSeriesDto(dateTimeTo, 1, 2, 2) };

            var returnsList = new List<TimeSeriesDto> { new TimeSeriesDto(dateTimeFrom, 1, 2, 2), new TimeSeriesDto(dateTimeFrom.AddDays(1), 1, 2, 2), new TimeSeriesDto(dateTimeFrom.AddDays(2), 0, 0, 0), new TimeSeriesDto(dateTimeFrom.AddDays(3), 0, 0, 0) };

            var data = CreateService();

            _shopRepository.Setup(sr => sr.GetByIdAsync(shopId, It.IsAny<CancellationToken>())).ReturnsAsync(shop);

            _analyticsCache.Setup(ac => ac.GetOrSetAsync<List<TimeSeriesDto>>(shopId, "time-series", $"{dateTimeFrom:O}:{dateTimeTo:O}", It.IsAny<Func<Task<List<TimeSeriesDto>>>>())).ReturnsAsync(DateList);

            var result = await data.TimeSeriesAsync(shopId, ownerUserId, dateTimeFrom, dateTimeTo, CancellationToken.None);

            Assert.Equal(returnsList, result);
        }

    }
}