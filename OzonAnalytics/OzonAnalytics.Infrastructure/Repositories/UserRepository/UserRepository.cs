using OzonAnalytics.Infrastructure.Database;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using Microsoft.EntityFrameworkCore;


namespace OzonAnalytics.Infrastructure.Repositories.UserRepository
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _db.ApplicationUsers.AnyAsync(u => u.Email == email);
        }
        
    }
}
