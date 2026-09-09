
namespace OzonAnalytics.Domain.Entities
{
    public class RefreshTokens
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string TokenHash { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool Revoked { get; private set;  }

        public RefreshTokens(Guid userId, string tokenHash)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = DateTime.UtcNow.AddDays(30);
            Revoked = false;    
        }
        public void Revoke() => Revoked = true;
        
        public bool IsActive => !Revoked && ExpiresAt > DateTime.UtcNow;
    }
}
