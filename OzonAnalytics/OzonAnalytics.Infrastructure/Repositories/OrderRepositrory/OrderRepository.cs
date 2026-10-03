using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories.OrderRepositrory
{
    public class OrderRepository: IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<Order>?> GetOrdersByShopId(Guid shopId, CancellationToken ct)
        {
            return await _db.Orders.Where(o => o.ShopId == shopId).ToListAsync(ct);
        }
        public async Task AddRangeAsync(List<Order> orders, CancellationToken ct)
        {
            await _db.Orders.AddRangeAsync(orders, ct);
            await _db.SaveChangesAsync(ct);
        }
        public async Task UpdateRangeAsync(List<Order> updatedTasks, CancellationToken ct)
        {
            _db.Orders.UpdateRange(updatedTasks);
            await _db.SaveChangesAsync(ct);
        }
    }
}
