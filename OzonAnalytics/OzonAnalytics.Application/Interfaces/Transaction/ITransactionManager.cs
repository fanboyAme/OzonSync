using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Transaction
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync(CancellationToken ct);

        Task CommitAsync(CancellationToken ct);

        Task RollbackAsync(CancellationToken ct);
        
    }
}
