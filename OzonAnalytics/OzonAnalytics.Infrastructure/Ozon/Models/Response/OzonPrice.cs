using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonPrice
    {
        public string Amount { get; set; } = default!;
        public string Currency { get; set; } = default!;
    }
}
