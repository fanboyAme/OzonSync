using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.ShopDtos
{
    public record class ConnectShopDto(string OzonClientId, string OzonApiKey, string ShopName);
}
