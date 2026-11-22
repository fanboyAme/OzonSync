using OzonAnalytics.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonPostingProduct
    {
        public string Name { get; set; } = default!;
        public long Sku {  get; set; }
        public string OfferId { get; set; } = default!;
        public int Quantity { get; set; }
        public OzonPrice Price { get; set; } = default!;
    }
}
