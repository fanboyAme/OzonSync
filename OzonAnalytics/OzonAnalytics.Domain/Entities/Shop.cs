using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Domain.Entities
{
    public class Shop
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public string OzonClientId { get; set; } = default!;
        public string OzonApiKeyEncrypted { get; set; } = default!;
        public DateTime OzonApiKeyExpiresAt { get; set; }
        public bool OzonApiKeyIsExpired { get; set; }
        public string Name { get; set; }
        public ShopStatus Status { get; set; }
        public DateTime? LastSyncedAt { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Shop(Guid userId, string ozonClientId, string ozonApiKeyEncrypted, string name, DateTime ozonApiKeyExpiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OzonClientId = ozonClientId;
            OzonApiKeyEncrypted = ozonApiKeyEncrypted;
            OzonApiKeyExpiresAt = ozonApiKeyExpiresAt;
            OzonApiKeyIsExpired = DateTime.UtcNow >= OzonApiKeyExpiresAt;
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
