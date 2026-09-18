using OzonAnalytics.Domain.Entities;


namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<bool> AddAsync(RefreshToken refreshToken, CancellationToken ct);
        public Task<bool> SaveChangesAsync(CancellationToken ct);
        
    }
}
