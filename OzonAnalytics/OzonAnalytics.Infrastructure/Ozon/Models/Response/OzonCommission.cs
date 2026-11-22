using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonCommission
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public decimal Percent { get; set; }
    }
}
