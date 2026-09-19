using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Shop;

namespace OzonAnalytics.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Shop")]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }
        
    }
}
