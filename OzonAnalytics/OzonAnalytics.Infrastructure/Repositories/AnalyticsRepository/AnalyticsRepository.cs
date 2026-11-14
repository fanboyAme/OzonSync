using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Domain.Enums;
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
            return _db.Orders.Where(o => o.ShopId == shopId && o.Status == OrderStatusAndSubStatus.Delivered && o.InProcessAt >= from && o.InProcessAt < to);
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
        public async Task<List<TopProductDto>> GetTopProductsAsync(Guid shopId, DateTime from, DateTime to, int limit, TopProductSortBy sortBy, CancellationToken ct)
        {
            var deliveredOrders = GetDeliveredOrders(from, to, shopId);

            var items = _db.OrderItems.Join(deliveredOrders, oi => oi.OrderId, o => o.Id, (oi, o) => oi);

            var finansialData = items.GroupBy(pi => pi.ProductId).Select(fd => new
            {
                Quantity = fd.Sum(q => q.Quantity),
                Revenue = fd.Sum(r => r.Quantity * r.Price),
                Payout = fd.Sum(p => p.Payout),
                ProductId = fd.Key
            });

            var product = _db.Products.Join(finansialData, p => p.Id, fd => fd.ProductId, (p, fd) => new { p.Id, p.OzonSku, p.Name, fd.Quantity, fd.Revenue, fd.Payout });

            switch (sortBy)
            {
                case TopProductSortBy.Items:
                    product = product.OrderByDescending(p => p.Quantity).ThenBy(p => p.Name);
                    break;
                case TopProductSortBy.Revenue:
                    product = product.OrderByDescending(p => p.Revenue).ThenBy(p => p.Name);
                    break;
                default:
                    product = product.OrderByDescending(p => p.Revenue).ThenBy(p => p.Name);
                    break;
            }
            var currentList = await product.Select(p => new TopProductDto(p.Id, p.OzonSku, p.Name, p.Quantity, p.Revenue, p.Payout)).Take(limit).ToListAsync(ct);

            return currentList;
        }
        public async Task<List<TimeSeriesDto>> GetTimeSeriesAsync(Guid shopId, DateTime from, DateTime to, CancellationToken ct)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            var deliveredOrders = GetDeliveredOrders(from, to, shopId);

            var orderDays = deliveredOrders.Select(dos => new
            {
                Id = dos.Id,
                Date = TimeZoneInfo.ConvertTimeFromUtc(dos.InProcessAt, tz).Date
            });
            var items = await _db.OrderItems.Join(orderDays, oi => oi.OrderId, o => o.Id, (oi, i) => new { oi.Payout, oi.Price, oi.Quantity, oi.OrderId, i.Date })
                .GroupBy(i => i.Date)
                .Select(i => new TimeSeriesDto
            (
                Date: i.Key,
                OrderCount: i.Select(oc => oc.OrderId).Distinct().Count(),
                Revenue: i.Sum(r => r.Quantity * r.Price),
                Payout: i.Sum(p => p.Payout)
            )).ToListAsync(ct);

            return items;
        }

    }
}
