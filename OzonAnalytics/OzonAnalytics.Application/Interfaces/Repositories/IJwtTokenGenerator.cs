using OzonAnalytics.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IJwtTokenGenerator
    {
        public string GenerateAccessToken(ApplicationUser applicationUser);
        public string GenerateRefreshToken();
        public string HashRefreshToken(string token);
    }
}
