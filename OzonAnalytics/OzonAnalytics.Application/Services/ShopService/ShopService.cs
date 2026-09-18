using OzonAnalytics.Application.Interfaces.Shop;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.RequestDtos;
using OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Services.ShopService
{
    public class ShopService: IShopService
    {
        //private

        //public ShopService()

        public async Task<ShopDto> ConnectShopAsync(ConnectShopDto connectShopDto, Guid userId)
        {
            //проверяем подключен ли входной магаз уже к пользователю
            // пробуем подключиться к озону с нашими данными ( ozonClient )
            // если ок, то шифруем ключ, создаем магазиг(также даем статус и последнее подключение по вермени) и сохраняем через репу
            // если нет, то бросаем ексепшн
        }
    }
}
