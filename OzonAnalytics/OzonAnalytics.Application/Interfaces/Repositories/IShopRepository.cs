using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IShopRepository
    {
        public Task<bool> AddAsync(Shop shop, Guid userId, CancellationToken ct);
        public Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken ct);
        public Task<List<Shop>?> GetShopByUserIdAsync(Guid userId, CancellationToken ct);
        public Task<bool> IsShopAlreadyConnectedAsync(Guid userId, string ozonClientId, CancellationToken ct);
    }
}
