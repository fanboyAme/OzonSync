using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.ShopDtos.RequestDtos
{
    public record class ConnectShopDto(string OzonClientId, string OzonApiKey, string ShopName);
}
