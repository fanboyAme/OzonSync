using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Auth;
using OzonAnalytics.Application.Interfaces.Shop;
using OzonAnalytics.Application.ProjectDtos.UserDtos;

namespace OzonAnalytics.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }
        
    }
}
