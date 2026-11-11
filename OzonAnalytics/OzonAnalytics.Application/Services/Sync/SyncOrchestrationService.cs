using Microsoft.Extensions.Logging;
using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Orders;
using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.Interfaces.Security;
using OzonAnalytics.Application.Interfaces.Transaction;
using OzonAnalytics.Application.ProjectDtos.SyncDTO;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Services.Sync
{
    public class SyncOrchestrationService : ISyncOrchestrationService
    {
        private readonly ILogger<SyncOrchestrationService> _logger;

        private readonly IOzonClient _client;

        private readonly IShopRepository _shopRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISyncLogRepository _syncLogRepository;

        private readonly IEncryptionService _encryptionService;

        private readonly ITransactionManager _transactionManager;

        public SyncOrchestrationService(IOzonClient ozonClient, IOrderRepository orderRepository, IShopRepository shopRepository, IEncryptionService encryption, IOrderItemRepository orderItemRepository, IProductRepository productRepository, ISyncLogRepository syncLogRepository, ILogger<SyncOrchestrationService> logger, ITransactionManager transactionManager)
        {
            _client = ozonClient;
            _orderRepository = orderRepository;
            _shopRepository = shopRepository;
            _encryptionService = encryption;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
            _syncLogRepository = syncLogRepository;
            _logger = logger;
            _transactionManager = transactionManager;
        }
        public async Task SyncShopAsync(Guid shopId, Guid userId, DateTime? since, int? limit, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var currentShop = await _shopRepository.GetByIdAsync(shopId, ct) ??
                throw new NotFoundException("Магазин не найден");
            if (currentShop.UserId != userId) throw new ForbiddenException("Доступ отказан");
            if (currentShop.Status == Domain.Enums.ShopStatus.Syncing) throw new ConflictException("Магазин уже синхронизируется");

            SyncLog? syncLog = null;

            try
            {

                currentShop.MarkSyncing();
                await _shopRepository.UpdateShopAsync(currentShop, ct);
                syncLog = new SyncLog(shopId, now);
                syncLog.InProcess();
                await _syncLogRepository.AddAsync(syncLog, ct);

                var apiKey = _encryptionService.Unprotect(currentShop.OzonApiKeyEncrypted);
                if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("Не удалось расшифровать ключ магазина");
                var clientId = currentShop.OzonClientId;

                var currentSince = since ?? currentShop.LastSyncedAt?.AddDays(-14) ?? now.AddDays(-30);
                var sinceTo = now;
                string? currentCursor = null;
                var currentLimit = limit ?? 50;
                var totalOrdersSynced = 0;

                var currentOrders = await _orderRepository.GetOrdersByShopId(shopId, ct);
                var existringOrdersDict = currentOrders.ToDictionary(co => co.OzonOrderId);

                var currentProducts = await _productRepository.GetProductByShopId(shopId, ct);
                var processedProduct = currentProducts.ToDictionary(cp => cp.OzonSku);
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
                            if (!processedProduct.TryGetValue(ozonItem.Sku, out var product))
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
                                null,
                                ozonItem.Payout
                            );
                            newOrderItem.Add(orderItem);
                        }
                    }
                    await _transactionManager.BeginTransactionAsync(ct);
                    try
                    {
                        if (newOrder.Count > 0) await _orderRepository.AddRangeAsync(newOrder, ct);
                        if (toUpdateOrder.Count > 0) await _orderRepository.UpdateRangeAsync(toUpdateOrder, ct);
                        if (newProduct.Count > 0) await _productRepository.AddRangeAsync(newProduct, ct);
                        if (newOrderItem.Count > 0) await _orderItemRepository.AddRangeAsync(newOrderItem, ct);
                        await _transactionManager.CommitAsync(ct);
                    }
                    catch
                    {
                        await _transactionManager.RollbackAsync(CancellationToken.None);
                        throw;
                    }


                    totalOrdersSynced += newOrder.Count + toUpdateOrder.Count;

                    foreach (var o in newOrder) existringOrdersDict[o.OzonOrderId] = o;

                    if (!result.HasNext || string.IsNullOrEmpty(result.Cursor)) break;

                    currentCursor = result.Cursor;
                }
                currentShop.MarkReady(now);
                await _shopRepository.UpdateShopAsync(currentShop, ct);

                syncLog.Complete(totalOrdersSynced);
                await _syncLogRepository.UpdateAsync(syncLog, ct);
            }
            catch (Exception ex)
            {
                try
                {
                    // сбрасываем состояние контекста после неуд SaveChanges
                    _shopRepository.ClearTracker();
                    currentShop.MarkError();
                    await _shopRepository.UpdateShopAsync(currentShop, CancellationToken.None);
                    if (syncLog != null)
                    {
                        syncLog.Fail(ex.Message);
                        await _syncLogRepository.UpdateAsync(syncLog, CancellationToken.None);
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "не удалось записать статус ошибки синхронизации магазина {ShopId}", currentShop.Id);
                }

                throw;
            }
        }
        public async Task<List<SyncLogDto>> GetLogsAsync(Guid shopId, int? limit, Guid userId, CancellationToken ct)
        {
            var currentShop = await _shopRepository.GetByIdAsync(shopId, ct) ?? throw new NotFoundException("Магазин с таким Id не найден");

            if (currentShop.UserId != userId) throw new ForbiddenException("Отказано в доступе");

            var currentLog = await _syncLogRepository.GetLogsByShopIdAsync(shopId, limit, ct);

            return currentLog.Select(sl => new SyncLogDto
            (
                sl.Id,
                shopId,
                sl.StartedAt,
                sl.FinishedAt,
                sl.Status,
                sl.OrdersSynced,
                sl.ErrorMessage
            )).ToList();
        }
    }
}
