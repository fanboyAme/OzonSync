using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IShopRepository
    {
        public Task AddAsync(Shop shop, CancellationToken ct);
        public Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken ct);
        public Task<List<Shop>?> GetShopByUserIdAsync(Guid userId, CancellationToken ct);
        public Task<Shop?> GetByUserIdAndClientIdAsync(Guid userId, string OzonClientId, CancellationToken ct);
    }
}
