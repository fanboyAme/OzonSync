using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Analytic;
using OzonAnalytics.Application.Interfaces.Auth;
using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/shops/{shopId}/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IFindCurrentUser _findCurrentUser;

        public AnalyticsController(IAnalyticsService analyticsService, IFindCurrentUser findCurrentUser)
        {
            _analyticsService = analyticsService;
            _findCurrentUser = findCurrentUser;
        }
        [HttpGet("summary")]
        public async Task<IActionResult> Summary(DateTime from, DateTime to, Guid shopId, CancellationToken ct)
        {
            var financialData = await _analyticsService.GetSummaryAsync(shopId, _findCurrentUser.FindCurrentUserId(), from, to, ct);
            return Ok(financialData);
        }
        [HttpGet("compare")]
        public async Task<IActionResult> Compare(DateTime from, DateTime to, Guid shopId, CancellationToken ct)
        {
            var compareData = await _analyticsService.CompareAsync(shopId, _findCurrentUser.FindCurrentUserId(), from, to, ct);
            return Ok(compareData);
        }
        [HttpGet("top-product")]
        public async Task<IActionResult> TopProduct(DateTime from, DateTime to, Guid shopId, int? limit, TopProductSortBy sortBy, CancellationToken ct)
        {
            var topProductList = await _analyticsService.TopProductAsync(shopId, _findCurrentUser.FindCurrentUserId(), from, to, limit, sortBy, ct);
            return Ok(topProductList);
        }
        [HttpGet("time-series")]
        public async Task<IActionResult> TimeSeries(Guid shopId, DateTime from, DateTime to, CancellationToken ct)
        {
            var timeSeriesData = await _analyticsService.TimeSeriesAsync(shopId, _findCurrentUser.FindCurrentUserId(), from, to, ct);
            return Ok(timeSeriesData);
        }
    }
}
