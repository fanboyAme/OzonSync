using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;
using OzonAnalytics.Infrastructure.Ozon.Models;

namespace OzonAnalytics.Application.Interfaces.Ozon
{
    public interface IOzonClient
    {
        public Task<OzonKeyValidationResult> ValidateCredentialsAsync(string clientId, string apiKey, CancellationToken ct);
        public Task<OzonOrdersPageResult> GetOrdersAsync(string clientId, string apiKey, DateTime since, DateTime to, string? cursor, int limit, CancellationToken ct);
    }
}
