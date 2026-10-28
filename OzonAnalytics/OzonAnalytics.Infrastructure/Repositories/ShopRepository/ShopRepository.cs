using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories.ShopRepository
{
    public class ShopRepository: IShopRepository
    {
        private readonly ApplicationDbContext _db;

        public ShopRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Shop shop, CancellationToken ct)
        {
            await _db.AddAsync(shop, ct);
            await _db.SaveChangesAsync(ct);
        }
        public async Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken ct)
        {
            return await _db.Shops.SingleOrDefaultAsync(s => s.Id == shopId, ct);
        }
        public async Task<List<Shop>?> GetShopsByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _db.Shops.Where(s => s.UserId == userId).ToListAsync(ct);
        }
        public async Task<Shop?> GetByUserIdAndClientIdAsync(Guid userId, string OzonClientId, CancellationToken ct)
        {
            return await _db.Shops.SingleOrDefaultAsync(s => s.UserId == userId && s.OzonClientId == OzonClientId, ct);
        }
        public async Task UpdateShopAsync(Shop shop, CancellationToken ct)
        {
            _db.Shops.Update(shop);
            await _db.SaveChangesAsync(ct);
        }
        public void ClearTracker()
        {
            _db.ChangeTracker.Clear();
        }
    }
}
