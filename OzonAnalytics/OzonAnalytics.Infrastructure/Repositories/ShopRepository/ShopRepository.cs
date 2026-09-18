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
        public async Task<bool> AddAsync(Shop shop, Guid userId, CancellationToken ct)
        {
            var newShop = new Shop
            (
                shop.UserId,
                shop.OzonClientId,
                shop.OzonApiKeyEncrypted,
                shop.Name
            );

            await _db.AddAsync(newShop, ct);
            
            return true;
        }
        public async Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken ct)
        {
            return await _db.Shops.SingleOrDefaultAsync(s => s.Id == shopId, ct);
        }
        public async Task<List<Shop>?> GetShopByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _db.Shops.Where(s => s.UserId == userId).ToListAsync(ct);
        }
        public async Task<bool> IsShopAlreadyExist(Guid userId, string ozonClientId, CancellationToken ct)
        {
            return await _db.Shops.AnyAsync(s => s.UserId == userId && s.OzonClientId == ozonClientId, ct);
        }
    }
}
