using OzonAnalytics.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Domain.Entities
{
    public class SyncLog
    {
        public Guid Id { get; private set; }
        public Guid ShopId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime FinishedAt { get; private set; }
        public SyncStatus Status { get; private set; }
        public int OrdersSynced { get; private set; }
        public string ErrorMessage { get; private set; } = default!;

        public SyncLog(Guid shopId, DateTime startedAt, DateTime finishedAt)
        {
            Id = Guid.NewGuid();
            ShopId = shopId;
            StartedAt = startedAt;
            Status = SyncStatus.InProcces;
        }
        public void Complete(int ordersSynced)
        {
            FinishedAt = DateTime.UtcNow;
            Status = SyncStatus.Success;
            OrdersSynced = ordersSynced;
        }
        public void Failes(string errorMessage)
        {
            FinishedAt = DateTime.UtcNow;
            Status = SyncStatus.Failed;
            ErrorMessage = errorMessage;
        }
    }
}
