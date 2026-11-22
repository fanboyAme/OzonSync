using OzonAnalytics.Application.ProjectDtos.ShopDtos.RequestDtos;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Interfaces.Shops
{
    public interface IShopService
    {
        public Task<ShopDto> ConnectShopAsync(ConnectShopDto connectShopDto, Guid userId, CancellationToken ct);
        public Task<Shop?> GetCurrentShopById(Guid shopId, Guid userId, CancellationToken ct);
        public Task<List<Shop>?> GetAllShopsByUserId(Guid userId, CancellationToken ct);
    }
}
