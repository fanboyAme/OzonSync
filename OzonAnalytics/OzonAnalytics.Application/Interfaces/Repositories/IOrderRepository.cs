using OzonAnalytics.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        public Task<List<Order>?> GetOrdersByShopId(Guid shopId, CancellationToken ct);
        public Task AddRangeAsync(List<Order> orders, CancellationToken ct);
        public Task UpdateRangeAsync(List<Order> updatedTasks, CancellationToken ct);
    }
}
