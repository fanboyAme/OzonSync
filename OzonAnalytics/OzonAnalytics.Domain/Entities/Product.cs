using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public long OzonSku { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public Product(Guid shopId, long ozonSku, string name, string category)
        {
            Id = Guid.NewGuid();
            ShopId = shopId;
            OzonSku = ozonSku;
            Name = name;
            Category = category;
        }
    }
}
