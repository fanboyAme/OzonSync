using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonFinancialData
    {
        public List<OzonFinancialProduct> Products { get; set; } = new();
    }
}
