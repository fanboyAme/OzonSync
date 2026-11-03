using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Auth;
using OzonAnalytics.Application.Interfaces.Orders;
using OzonAnalytics.Application.ProjectDtos.SyncDTO;

namespace OzonAnalytics.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/shops/{shopId}")]
    public class SyncController : ControllerBase
    {
        private readonly ISyncOrchestrationService _syncOrchestrationService;
        private readonly IFindCurrentUser _findCurrentUser;
        public SyncController(ISyncOrchestrationService syncOrchestrationService, IFindCurrentUser findCurrentUser)
        {
            _syncOrchestrationService = syncOrchestrationService;
            _findCurrentUser = findCurrentUser;
        }

        [HttpPost("sync")]
        // Добавить 202, так как синхронизация может быть долгой, а обычный ct отменит с флагом Failed.
        // Для этого тут нужно будет оставить обычный 202, и отдать задачу воркеру, который будет ее выполнять (саму синхронизацию).
        public async Task<IActionResult> Sync(Guid shopId, DateTime? since, int? limit, CancellationToken ct)
        {
            await _syncOrchestrationService.SyncShopAsync(shopId, _findCurrentUser.FindCurrentUserId(), since, limit, ct);
            return Ok();
        }
        [HttpGet("logs")]
        public async Task<ActionResult<List<SyncLogDto>>> GetLogs(Guid shopId, int? limit, CancellationToken ct)
        {
            var logs = await _syncOrchestrationService.GetLogsAsync(shopId, limit, _findCurrentUser.FindCurrentUserId(), ct);
            return Ok(logs);
        }
    }
}
