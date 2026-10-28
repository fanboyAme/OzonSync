using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models.Request
{
    public class OzonPostingsFilter
    {
        public DateTime Since { get; set; }
        public DateTime To { get; set; }
    }
}
