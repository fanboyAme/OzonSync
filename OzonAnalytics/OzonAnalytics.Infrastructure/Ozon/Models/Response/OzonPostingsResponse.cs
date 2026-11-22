using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Response
{
    public class OzonPostingsResponse
    {
        public string Cursor { get; set; } = default!;
        public bool HasNext { get; set; }
        public List<OzonPosting> Postings { get; set; } = new();
    }
}
