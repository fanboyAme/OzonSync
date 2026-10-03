using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;
using OzonAnalytics.Infrastructure.Ozon.Models;
using OzonAnalytics.Infrastructure.Ozon.Models.Request;
using OzonAnalytics.Infrastructure.Ozon.Models.Response;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
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
        public async Task<OzonKeyValidationResult> ValidateCredentialsAsync(string clientId, string apiKey, CancellationToken ct)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-seller.ozon.ru/v1/roles");
            request.Headers.Add("Client-Id", clientId);
            request.Headers.Add("Api-Key", apiKey);

            try
            {
            var parse = await SendAndParseAsync<OzonRolesResponse>(request, ct);

            var hasOrderAccess = parse!.Roles.Any(r => r.Methods.Any(m => m.Contains("/v1/posting")));

            return new OzonKeyValidationResult(
              IsValid: hasOrderAccess && !(parse.ExpiresAt <= DateTime.UtcNow),
              ExpiresAt: parse.ExpiresAt
              );
            }
            catch(Exception ex) when(ex is UnauthorizedException or NotFoundException)  
            {
                return new OzonKeyValidationResult(false, null);
            }
        }
        public async Task<OzonOrdersPageResult> GetOrdersAsync(string clientId, string apiKey, DateTime since, DateTime to, string? cursor, int limit, CancellationToken ct)
        {
            var requestBody = new OzonPostingsRequest
            {
                Dir = "asc",
                Filter = new OzonPostingsFilter { Since = since, To = to  },
                Limit = limit,
                Cursor = cursor ?? "",
                With = new OzonPostingsWith { FinancialData = true  },
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-seller.ozon.ru/v4/posting/fbs/list");

            request.Headers.Add("Client-Id", clientId);
            request.Headers.Add("Api-Key", apiKey);

            request.Content = JsonContent.Create(requestBody, options: _jsonSerializerOptions);

            var parsed = await SendAndParseAsync<OzonPostingsResponse>(request, ct);

            var orders = parsed.Postings.Select(posting => new OzonOrderData
            {
                PostingNumber = posting.PostingNumber,
                Status = posting.Status,
                Substatus = posting.Substatus,
                InProcessAt = posting.InProcessAt,
                Items = posting.Products.Select(product =>
                {
                    var finansialData = posting.FinancialData.Products.FirstOrDefault(fd => fd.ProductId == product.Sku);

                    return new OzonOrderItemData
                    {
                        Sku = product.Sku,
                        Name = product.Name,
                        Quantity = product.Quantity,
                        Price = finansialData?.Price ?? 0,
                        CommissionAmount = finansialData?.Commission.Amount ?? 0,
                        Payout = finansialData?.Payout ?? 0
                    };
                }).ToList()
            }).ToList();


            return new OzonOrdersPageResult
            {
                Cursor = parsed.Cursor,
                HasNext = parsed.HasNext,
                Orders = orders
            };
        }
        private async Task<T?> SendAndParseAsync<T>(HttpRequestMessage request, CancellationToken ct) where T : class
        {
            var response = await _httpClient.SendAsync(request, ct);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) throw new UnauthorizedException("Неверные ключи Озон");

            var jsonString = await response.Content.ReadAsStringAsync(ct);

            var parse = JsonSerializer.Deserialize<T>(jsonString, _jsonSerializerOptions) ?? 
                throw new NotFoundException("Не удалось распарсить ответ от Озон");

            return parse;
        }
    }
}
