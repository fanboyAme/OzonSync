using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        public Task AddRangeAsync(List<Product> products, CancellationToken ct);
        public Task<List<Product>> GetProductByShopId(Guid shopId, CancellationToken ct);
    }
}
