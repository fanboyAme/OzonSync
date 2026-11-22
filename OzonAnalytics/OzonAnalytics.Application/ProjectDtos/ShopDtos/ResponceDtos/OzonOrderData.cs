using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.ShopDtos.ResponceDtos
{
    public class OzonOrderData
    {
        public string PostingNumber { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Substatus {  get; set; } = default!;
        public DateTime InProcessAt { get; set; }
        public List<OzonOrderItemData> Items { get; set; } = new ();
    }
}
