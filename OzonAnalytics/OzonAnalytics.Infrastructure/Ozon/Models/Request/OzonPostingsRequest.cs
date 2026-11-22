using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Request
{
    public class OzonPostingsRequest
    {
        public string Dir {  get; set; } = default!;
        public OzonPostingsFilter Filter { get; set; } = default!;
        public int Limit { get; set; }
        public string Cursor { get; set; } = default!;
        public OzonPostingsWith With { get; set; } = default!;
    }
}
