using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos
{
    public class OzonOrderItemData
    {
        public long Sku {  get; set; }
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal Payout {  get; set; }
    }
}
