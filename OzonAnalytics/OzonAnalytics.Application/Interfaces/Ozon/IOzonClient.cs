using OzonAnalytics.Infrastructure.Ozon.Models;

namespace OzonAnalytics.Application.Interfaces.Ozon
{
    public interface IOzonClient
    {
        public Task<OzonKeyValidationResult> ValidateCredentialsAsync(string clientId, string apiKey);
    }
}
