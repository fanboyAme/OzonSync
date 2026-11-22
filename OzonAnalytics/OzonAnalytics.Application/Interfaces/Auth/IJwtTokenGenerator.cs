using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        public string GenerateAccessToken(ApplicationUser applicationUser);
        public string GenerateRefreshToken();
        public string HashRefreshToken(string token);
    }
}
