using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Analytic;
using OzonAnalytics.Application.Interfaces.Auth;

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

    }
}
