using Microsoft.AspNetCore.Identity;

namespace OzonAnalytics.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
