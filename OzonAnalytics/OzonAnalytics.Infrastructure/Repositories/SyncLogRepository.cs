using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories
{
    public class SyncLogRepository: ISyncLogRepository
    {
        private readonly ApplicationDbContext _db;

        public SyncLogRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(SyncLog syncLog, CancellationToken ct)
        {
            await _db.AddAsync(syncLog, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
