using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;
using OzonAnalytics.Infrastructure.Ozon.Models;

namespace OzonAnalytics.Infrastructure.Ozon.Fake
{
    public class FakeOzonClient : IOzonClient
    {
        public async Task<OzonKeyValidationResult> ValidateCredentialsAsync(string clientId, string apiKey, CancellationToken ct)
        {
            if (clientId == "admintest" && apiKey == "admintest")
            {
                return new OzonKeyValidationResult(
                                 IsValid: true,
                                 ExpiresAt: DateTime.Now.AddYears(1)
                                 );
            }
            else return new OzonKeyValidationResult(
                                 IsValid: false,
                                 ExpiresAt: DateTime.Now
                                 );

        }
        private static List<OzonOrderData> BuildOrders(int count)
        {
            // три повторяющихся товара, чтобы проверить дубли SKU
            var catalog = new[]
            {
        (Sku: 1904686181L, Name: "Смартфон Xiaomi Note 12", Price: 12990m),
        (Sku: 1904686182L, Name: "Наушники Bluetooth",      Price: 3490m),
        (Sku: 1904686183L, Name: "Чехол для смартфона",     Price: 590m),
    };

            // статусы Ozon (подстатусы сверьте со своим списком статусов)
            var statuses = new[]
            {
        ("awaiting_packaging", "posting_created"),
        ("delivering",         "posting_in_carriage"),
        ("delivered",          "posting_received"),
        ("cancelled",          "posting_canceled"),
    };

            var orders = new List<OzonOrderData>();

            for (var i = 0; i < count; i++)
            {
                var (status, substatus) = statuses[i % statuses.Length];

                // каждый третий заказ содержит два товара
                var itemsCount = i % 3 == 0 ? 2 : 1;
                var items = new List<OzonOrderItemData>();

                for (var j = 0; j < itemsCount; j++)
                {
                    var product = catalog[(i + j) % catalog.Length];
                    var commission = Math.Round(product.Price * 0.1m, 2);

                    items.Add(new OzonOrderItemData
                    {
                        Sku = product.Sku,
                        Name = product.Name,
                        Quantity = 1 + (i % 2),
                        Price = product.Price,
                        CommissionAmount = commission,
                        Payout = product.Price - commission - 150m, // 150 — условные прочие удержания
                    });
                }

                orders.Add(new OzonOrderData
                {
                    PostingNumber = $"0208245774-{i + 1:0000}-1",
                    Status = status,
                    Substatus = substatus,
                    InProcessAt = DateTime.UtcNow.AddDays(-(count - i)),
                    Items = items,
                });
            }

            return orders;
        }
        private static int CursorOrders(string? cursor)
        {
            if (cursor == null) return 0;
            return int.Parse(cursor);
        }
        public Task<OzonOrdersPageResult> GetOrdersAsync(string clientId, string apiKey, DateTime since, DateTime to, string? cursor, int limit, CancellationToken ct)
        {
            var orders = BuildOrders(9);

            var filterOrders = orders.Where(o => o.InProcessAt > since && o.InProcessAt < to).ToList();

            var start = CursorOrders(cursor);

            var takeOrders = filterOrders.Skip(start).Take(limit).ToList();

            var nextPosition = start + takeOrders.Count;

            var hasNext = nextPosition < filterOrders.Count;

            var nextCursor = hasNext ? nextPosition.ToString() : "";

            return Task.FromResult(new OzonOrdersPageResult
            {
                Cursor = nextCursor,
                HasNext = hasNext,
                Orders = takeOrders
            });
        }
    }
}

