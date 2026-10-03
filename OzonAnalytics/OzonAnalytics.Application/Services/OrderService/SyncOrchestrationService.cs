using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Orders;
using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.Interfaces.Security;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Services.OrderService
{
    public class SyncOrchestrationService : ISyncOrchestrationService
    {
        private readonly IOzonClient _client;

        private readonly IShopRepository _shopRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISyncLogRepository _syncLogRepository;
        
        private readonly IEncryptionService _encryptionService;

        public SyncOrchestrationService(IOzonClient ozonClient, IOrderRepository orderRepository, IShopRepository shopRepository, IEncryptionService encryption, IOrderItemRepository orderItemRepository, IProductRepository productRepository, ISyncLogRepository syncLogRepository)
        {
            _client = ozonClient;
            _orderRepository = orderRepository;
            _shopRepository = shopRepository;
            _encryptionService = encryption;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
            _syncLogRepository = syncLogRepository;
        }
        public async Task SyncShopAsync(Guid shopId, Guid userId, DateTime? since, int? limit, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            var startedAt = now;
            var currentShop = await _shopRepository.GetByIdAsync(shopId, ct) ??
                throw new NotFoundException("Магазин не найден");
            if (currentShop.UserId != userId) throw new ForbiddenException("Доступ отказан");

            currentShop.MarkSyncing();
            await _shopRepository.UpdateShopAsync(currentShop, ct);
            var syncLog = new SyncLog(shopId, now);
            syncLog.InProcess();
            await _syncLogRepository.AddAsync(syncLog, ct);


            var apiKey = _encryptionService.Unprotect(currentShop.OzonApiKeyEncrypted);
            var clientId = currentShop.OzonClientId;

            var currentSince = since ?? currentShop.LastSyncedAt ?? now.AddDays(-30);
            var sinceTo = now;
            string? currentCursor = null;
            var currentLimit = limit ?? 50;
            var totalOrdersSynced = 0;

            var currentOrders = await _orderRepository.GetOrdersByShopId(shopId, ct) ?? new List<Order>();
            var existringOrdersDict = currentOrders.ToDictionary(co => co.OzonOrderId);

            var processedProduct = new Dictionary<long, Product>();

            try
            {
            while (true)
                        {
                            var result = await _client.GetOrdersAsync(clientId, apiKey, currentSince, sinceTo, currentCursor, currentLimit, ct);

                            if (result.Orders == null || result.Orders.Count == 0) break;

                            var newOrder = new List<Order>();

                            var toUpdateOrder = new List<Order>();

                            var newOrderItem = new List<OrderItem>();

                            var newProduct = new List<Product>();

                            foreach (var orders in result.Orders)
                            {
                                if (existringOrdersDict.TryGetValue(orders.PostingNumber, out var existingOrder))
                                {
                                    if (existingOrder.Status != orders.Status || existingOrder.Substatus != orders.Substatus)
                                    {
                                        existingOrder.Status = orders.Status;
                                        existingOrder.Substatus = orders.Substatus;

                                        toUpdateOrder.Add(existingOrder);
                                    }
                                    continue;
                                }

                                var order = new Order
                                (
                                    shopId,
                                    orders.PostingNumber,
                                    orders.Status,
                                    orders.Substatus,
                                    now,
                                    orders.InProcessAt
                                );

                                newOrder.Add(order);

                                foreach (var ozonItem in orders.Items)
                                {
                                    if(!processedProduct.TryGetValue(ozonItem.Sku, out var product))
                                    {
                                        product = new Product
                                        (
                                            shopId,
                                            ozonItem.Sku,
                                            ozonItem.Name,
                                            null
                                        );

                                        processedProduct[ozonItem.Sku] = product;

                                        newProduct.Add(product);
                                    }

                                    var orderItem = new OrderItem
                                    (
                                        order.Id,
                                        product.Id,
                                        ozonItem.Quantity,
                                        ozonItem.Price,
                                        ozonItem.CommissionAmount,
                                        (ozonItem?.Payout ?? 0) - (ozonItem?.Price ?? 0) - (ozonItem?.CommissionAmount ?? 0),
                                        ozonItem.Payout
                                    );
                                    newOrderItem.Add(orderItem);
                                }
                            }
                            if (newOrder.Count > 0) await _orderRepository.AddRangeAsync(newOrder, ct);
                            if (toUpdateOrder.Count > 0) await _orderRepository.UpdateRangeAsync(toUpdateOrder, ct);
                            if (newProduct.Count > 0) await _productRepository.AddRangeAsync(newProduct, ct);
                            if (newOrderItem.Count > 0) await _orderItemRepository.AddRangeAsync(newOrderItem, ct);

                            totalOrdersSynced += newOrder.Count + toUpdateOrder.Count;

                            foreach (var o in newOrder) existringOrdersDict[o.OzonOrderId] = o;

                            if (!result.HasNext || string.IsNullOrEmpty(result.Cursor)) break;

                            currentCursor = result.Cursor;
                        }
            currentShop.MarkReady(now);
            await _shopRepository.UpdateShopAsync(currentShop, ct);

            syncLog.Complete(totalOrdersSynced);
            await _syncLogRepository.AddAsync(syncLog, ct);
            }
            catch(Exception ex)
            {
                currentShop.MarkError();
                await _shopRepository.UpdateShopAsync(currentShop, ct);
                syncLog.Failes(ex.Message);
                await _syncLogRepository.AddAsync(syncLog, ct);

                throw;
            }
        } 
    }
}
