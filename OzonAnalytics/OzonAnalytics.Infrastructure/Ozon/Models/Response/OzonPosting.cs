using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonPosting
    {
        public int OrderId { get; set; }
        public string PostingNumber { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Substatus { get; set; } = default!;
        public DateTime InProcessAt { get; set; }
        public List<OzonPostingProduct> Products { get; set; } = new();
        public OzonFinancialData FinancialData { get; set; } = default!;


    }
}
