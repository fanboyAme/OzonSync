using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories
{
    public class SyncLogRepository : ISyncLogRepository
    {
        private readonly ApplicationDbContext _db;

        public SyncLogRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(SyncLog syncLog, CancellationToken ct)
        {
            await _db.SyncLogs.AddAsync(syncLog, ct);
            await _db.SaveChangesAsync(ct);
        }
        public async Task UpdateAsync(SyncLog syncLog, CancellationToken ct)
        {
            _db.SyncLogs.Update(syncLog);
            await _db.SaveChangesAsync(ct);
        }
        public async Task<List<SyncLog>> GetLogsByShopIdAsync(Guid shopId, int? limit, CancellationToken ct)
        {
            var take = Math.Clamp(limit ?? 10, 1, 50);
            return await _db.SyncLogs.Where(sl => sl.ShopId == shopId).OrderByDescending(sl => sl.StartedAt).Take(take).ToListAsync(ct);
        }
    }
}
