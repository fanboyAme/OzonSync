using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Ozon
{
    public interface IOzonClient
    {
        public Task<bool> ValidateCredentialsAsync(string clientId, string apiKey);
    }
}
