using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories.OrderItemRepository
{
    public class OrderItemRepository: IOrderItemRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderItemRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddRangeAsync(List<OrderItem> ordersItem, CancellationToken ct)
        {
            await _db.OrderItems.AddRangeAsync(ordersItem, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}