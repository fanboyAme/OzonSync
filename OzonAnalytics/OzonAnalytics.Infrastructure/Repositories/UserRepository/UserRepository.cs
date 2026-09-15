using OzonAnalytics.Infrastructure.Database;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Domain.Entities;


namespace OzonAnalytics.Infrastructure.Repositories.UserRepository
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> AddAsync(RefreshToken refreshToken)
        {
            await _db.RefreshTokens.AddAsync(refreshToken);
            return true;
        }
        public async Task<bool> SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
