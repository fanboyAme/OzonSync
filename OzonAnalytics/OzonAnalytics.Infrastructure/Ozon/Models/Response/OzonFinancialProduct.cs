using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonFinancialProduct
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }  
        public decimal Payout { get; set; }
        public int Quantity { get; set; }
        public OzonCommission Commission { get; set; } = default!;
    }
}
