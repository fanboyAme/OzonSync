using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos
{
    public class OzonOrdersPageResult
    {
        public string Cursor { get; set; } = default!;
        public bool HasNext { get; set; }
        public List<OzonOrderData> Orders { get; set; } = new();
    }
}
