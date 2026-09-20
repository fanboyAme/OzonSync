using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Shops;

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
        //1. connecting shop POST?
        //2. get current shop by id GET
        //3. get current list shop by userid GET
    }
}
