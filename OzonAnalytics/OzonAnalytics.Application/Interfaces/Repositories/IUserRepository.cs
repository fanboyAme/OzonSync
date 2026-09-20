using OzonAnalytics.Domain.Entities;


namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task AddAsync(RefreshToken refreshToken, CancellationToken ct);
    }
}
