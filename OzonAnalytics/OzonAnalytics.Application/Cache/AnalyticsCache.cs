using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OzonAnalytics.Application.Interfaces.Cache;
using OzonAnalytics.Application.Options;
using System.Text.Json;

namespace OzonAnalytics.Application.Cache
{
    public class AnalyticsCache
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<AnalyticsCache> _logger;
        private readonly AnalyticsCacheOptions _analyticsCacheOptions;
        public AnalyticsCache(ICacheService cacheService, ILogger<AnalyticsCache> logger, IOptions<AnalyticsCacheOptions> analyticsCacheOptions)
        {
            _cacheService = cacheService;
            _logger = logger;
            _analyticsCacheOptions = analyticsCacheOptions.Value;
        }

        public async Task<T> GetOrSetAsync<T>(Guid shopId, string method, string parametr, Func<Task<T>> factory)
        {
            long versionShop = 0;
            string? key = null;
            string? value = null;
            try
            {
                versionShop = await _cacheService.GetVersionAsync(shopId);
                key = _cacheService.SetKey(shopId, versionShop, method, parametr);
                value = await _cacheService.GetValueByKey(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Кэш недоступен при чтении метода: {method} магазина: {shopId}", method, shopId);
            }

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    var deserializeData = JsonSerializer.Deserialize<T>(value);
                    if (deserializeData is not null)
                    {
                        return deserializeData;
                    }
                }
                catch (JsonException ex)
                {
                    //ошибка может быть если данные DTO изменились, или битый кэш
                    _logger.LogWarning(ex, "Ошибка десериализации данных магазина {shopId}, проверьте DTO", shopId);
                }
            }
            var data = await factory();
            if (key != null)
            {
                try
                {
                    await _cacheService.SetValueAsync(key, JsonSerializer.Serialize(data), TimeSpan.FromMinutes(_analyticsCacheOptions.AnalyticsTtlMinutes));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось записать в кэш ключ: {key}", key);

                }
            }
            return data;
        }
    }
}
