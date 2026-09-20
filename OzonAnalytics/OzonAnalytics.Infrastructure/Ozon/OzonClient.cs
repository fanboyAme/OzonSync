using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Infrastructure.Ozon.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OzonAnalytics.Infrastructure.Ozon
{
    public class OzonClient: IOzonClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        public OzonClient (HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<OzonKeyValidationResult> ValidateCredentialsAsync(string clientId, string apiKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-seller.ozon.ru/v1/roles");
            request.Headers.Add("Client-Id", clientId);
            request.Headers.Add("Api-Key", apiKey);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return new OzonKeyValidationResult(false, null);

            var jsonString = await response.Content.ReadAsStringAsync();

            var parse = JsonSerializer.Deserialize<OzonRolesResponse>(jsonString, _jsonSerializerOptions);

            if (parse is null) return new OzonKeyValidationResult(false, null);

            var hasOrderAccess = parse.Roles.Any(r => r.Methods.Any(m => m.Contains("/v1/posting")));

            return new OzonKeyValidationResult(
                IsValid: hasOrderAccess && !(parse.ExpiresAt <= DateTime.UtcNow),
                ExpiresAt: parse.ExpiresAt
                );

        }
    }
}
