using OzonAnalytics.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos
{
    public record class ShopDto(Guid Id, string Name, ShopStatus Status, DateTime LastSyncedAt);
}
