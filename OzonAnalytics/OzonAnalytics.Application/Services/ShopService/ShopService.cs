using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.Interfaces.Security;
using OzonAnalytics.Application.Interfaces.Shops;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.RequestDtos;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Services.ShopService
{
    public class ShopService : IShopService
    {
        private readonly IOzonClient _client;
        private readonly IShopRepository _shopRepository;
        private readonly IEncryptionService _encryptionService;

        public ShopService(IOzonClient client, IShopRepository shopRepository, IEncryptionService encryptionService)
        {
            _client = client;
            _shopRepository = shopRepository;
            _encryptionService = encryptionService;
        }

        public async Task<ShopDto> ConnectShopAsync(ConnectShopDto connectShopDto, Guid userId, CancellationToken ct)
        {
            //проверяем подключен ли входной магаз уже к пользователю
            var currentShop = await _shopRepository.GetByUserIdAndClientIdAsync(userId, connectShopDto.OzonClientId, ct);

            if (currentShop is not null)
            {
                return new ShopDto
                (
                    currentShop!.Id,
                    currentShop.Name,
                    currentShop.Status,
                    currentShop.LastSyncedAt
                );
            }
            // пробуем подключиться к озону с нашими данными ( ozonClient )

            var connectToOzon = await _client.ValidateCredentialsAsync(connectShopDto.OzonClientId, connectShopDto.OzonApiKey, ct);

            if (!connectToOzon.IsValid) throw new ValidationException("Неверные ключи Ozon или истекший срок их действия");

            if (connectToOzon.ExpiresAt is null)
            {
                throw new ValidationException("Не удалось определить срок действия ключа");
            }

            var createdShop = new Shop
                (
                userId,
                connectShopDto.OzonClientId,
                _encryptionService.Protect(connectShopDto.OzonApiKey),
                connectShopDto.ShopName,
                connectToOzon.ExpiresAt.Value
                );
            await _shopRepository.AddAsync(createdShop, ct);

            return new ShopDto(createdShop.Id, createdShop.Name, createdShop.Status, createdShop.LastSyncedAt);
        }
        public async Task<Shop?> GetCurrentShopById(Guid shopId, Guid userId, CancellationToken ct)
        {
            var currentShop = await _shopRepository.GetByIdAsync(shopId, ct);

            if(currentShop is null) throw new NotFoundException("Магазин не найден");
            if (currentShop.UserId != userId) throw new ForbiddenException("Нет доступа к магазину");

            return currentShop;
        }
        public async Task<List<Shop>?> GetAllShopsByUserId(Guid userId, CancellationToken ct)
        {
            return await _shopRepository.GetShopsByUserIdAsync(userId, ct);
        }
    }
}
