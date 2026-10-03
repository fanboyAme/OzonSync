using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories.ProductRepository
{
    public class ProductRepository: IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddRangeAsync(List<Product> products, CancellationToken ct)
        {
            await _db.Products.AddRangeAsync(products, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
