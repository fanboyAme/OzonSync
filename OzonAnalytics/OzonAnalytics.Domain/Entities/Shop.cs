using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Domain.Entities
{
    public class Shop
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string OzonClientId { get; private set; } = default!;
        public string OzonApiKeyEncrypted { get; private set; } = default!;
        public string Name { get; private set; }
        public ShopStatus Status { get; private set; }
        public DateTime LastSyncedAt { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Shop(Guid userId, string ozonClientId, string ozonApiKeyEncrypted, string name)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OzonClientId = ozonClientId;
            OzonApiKeyEncrypted = ozonApiKeyEncrypted;
            Name = name;
        }
        public void MarkSyncing() => Status = ShopStatus.Syncing;

        public void MarkReady(DateTime syncedAt)
        {
            Status = ShopStatus.Ready;
            LastSyncedAt = syncedAt;
        }
        public void MarkError () => Status = ShopStatus.Error;
    }
}
