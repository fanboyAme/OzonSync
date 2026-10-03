using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Auth;
using OzonAnalytics.Application.Interfaces.Shops;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.RequestDtos;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Api.Controllers
{
    [ApiController]
    [Route("api/v1/shops")]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;
        private readonly IFindCurrentUser _findCurrentUser;

        public ShopController(IShopService shopService, IFindCurrentUser findCurrentUser)
        {
            _shopService = shopService;
            _findCurrentUser = findCurrentUser;
            
        }
        [HttpPost("connect")]
        [Authorize]
        public async Task<IActionResult> ConnectShopAsync(ConnectShopDto connectShopDto, CancellationToken ct)
        {
            var result = await _shopService.ConnectShopAsync(connectShopDto, _findCurrentUser.FindCurrentUserId(), ct);
            return Ok(result);
        }
        [HttpGet("{shopId}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentShopById(Guid shopId, CancellationToken ct)
        {
            var result = await _shopService.GetCurrentShopById(shopId, _findCurrentUser.FindCurrentUserId(), ct);
            return Ok(result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllShopsByUserId(CancellationToken ct)
        {
            var result = await _shopService.GetAllShopsByUserId(_findCurrentUser.FindCurrentUserId(), ct);
            return Ok(result);
        }
        
    }
}
