using OzonAnalytics.Infrastructure.Database;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Application.Interfaces.Repositories;


namespace OzonAnalytics.Infrastructure.Repositories.UserRepository
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> AddAsync(RefreshToken refreshToken, CancellationToken ct)
        {
            await _db.RefreshTokens.AddAsync(refreshToken, ct);
            return true;
        }
        public async Task<bool> SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
