using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Domain.Entities
{
    public class SyncLog
    {
        public Guid Id { get; private set; }
        public Guid ShopId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? FinishedAt { get; private set; }
        public SyncStatus Status { get; private set; }
        public int OrdersSynced { get; private set; }
        public string? ErrorMessage { get; private set; } = default!;

        public SyncLog(Guid shopId, DateTime startedAt)
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
        public void InProcess()
        {
            FinishedAt = null;
            Status = SyncStatus.InProcces;
            OrdersSynced = 0;
        }
        public void Fail(string errorMessage)
        {
            FinishedAt = DateTime.UtcNow;
            Status = SyncStatus.Failed;
            ErrorMessage = errorMessage;
        }
    }
}
