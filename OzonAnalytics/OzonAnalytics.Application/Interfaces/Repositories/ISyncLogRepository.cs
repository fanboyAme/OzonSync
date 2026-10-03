using OzonAnalytics.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface ISyncLogRepository
    {
        public Task AddAsync(SyncLog syncLog, CancellationToken ct);
    }
}
