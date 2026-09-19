using OzonAnalytics.Application.ProjectDtos.ShopDtos.RequestDtos;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;

namespace OzonAnalytics.Application.Interfaces.Shops
{
    public interface IShopService
    {
        public Task<ShopDto> ConnectShopAsync(ConnectShopDto connectShopDto, Guid userId);
    }
}
