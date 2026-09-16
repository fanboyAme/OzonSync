using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public int TimeAlive { get; set; }
    }
}
