using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Database;

namespace OzonAnalytics.Infrastructure.Repositories.AnalyticsRepository
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly ApplicationDbContext _db;
        public AnalyticsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        private IQueryable<Order> GetDeliveredOrders(DateTime from, DateTime to, Guid shopId)
        {
            return _db.Orders.Where(o => o.ShopId == shopId && o.Status == "delivered" && o.InProcessAt >= from && o.InProcessAt < to);
        }
        private IQueryable<Order> GetOrdersWithDateFilter(DateTime from, DateTime to, Guid shopId)
        {
            return _db.Orders.Where(o => o.ShopId == shopId && o.InProcessAt >= from && o.InProcessAt < to);
        }
        public async Task<SummaryResponseDto> GetFinancialSummaryAsync(DateTime from, DateTime to, Guid shopId, CancellationToken ct)
        {
            var orders = GetOrdersWithDateFilter(from, to, shopId);
            var deliveredOrders = GetDeliveredOrders(from, to, shopId);

            var items = _db.OrderItems.Join(deliveredOrders, oi => oi.OrderId, o => o.Id, (oi, o) => oi);

            var finansialData = await items.GroupBy(_ => 1).Select(fd => new
            {
                ItemsSold = fd.Sum(isd => isd.Quantity),
                Revenue = fd.Sum(r => r.Quantity * r.Price),
                Commission = fd.Sum(c => c.CommissionAmount),
                Payout = fd.Sum(p => p.Payout)
            })
            .FirstOrDefaultAsync(ct);

            var orderCount = await deliveredOrders.CountAsync(ct);
            var itemsSold = finansialData?.ItemsSold ?? 0;
            var revenue = finansialData?.Revenue ?? 0;
            var commission = finansialData?.Commission ?? 0;
            var payout = finansialData?.Payout ?? 0;
            var averageCheck = orderCount > 0 ? revenue / orderCount : 0m;
            var statusBreakdown = await orders.GroupBy(s => s.Status).Select(o => new StatusCountDto(Status: o.Key, Count: o.Count())).ToListAsync(ct);

            return new SummaryResponseDto
            (
                OrderCount: orderCount,
                ItemsSold: itemsSold,
                Revenue: revenue,
                Commission: commission,
                Payout: payout,
                AverageCheck: averageCheck,
                StatusBreakdown: statusBreakdown
            );
        }
    }
}
