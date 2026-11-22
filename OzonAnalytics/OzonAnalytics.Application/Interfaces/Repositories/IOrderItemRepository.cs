using OzonAnalytics.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IOrderItemRepository
    {
        public Task AddRangeAsync(List<OrderItem> ordersItem, CancellationToken ct);
    }
}
