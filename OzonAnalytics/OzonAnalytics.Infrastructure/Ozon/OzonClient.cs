using OzonAnalytics.Application.Interfaces.Ozon;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon
{
    public class OzonClient: IOzonClient
    {
        public async Task<bool> ValidateCredentialsAsync(string clientId, string apiKey)
        {
            return true;
        }
    }
}
