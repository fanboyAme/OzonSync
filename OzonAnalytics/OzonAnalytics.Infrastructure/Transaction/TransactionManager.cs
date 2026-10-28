using OzonAnalytics.Application.Interfaces.Transaction;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Transaction
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ApplicationDbContext _db;

        public TransactionManager(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            await _db.Database.BeginTransactionAsync(ct);
        }
        public async Task CommitAsync(CancellationToken ct)
        {
            await _db.Database.CommitTransactionAsync(ct);
        }
        public async Task RollbackAsync(CancellationToken ct)
        {
            await _db.Database.RollbackTransactionAsync(ct);
        }
    }
}
