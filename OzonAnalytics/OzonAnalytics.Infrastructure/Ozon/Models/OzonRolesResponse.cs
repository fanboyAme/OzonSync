using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models
{
    public class OzonRolesResponse
    {
        public DateTime ExpiresAt;
        public List<OzonRole> Roles { get; set; } = new List<OzonRole>();
    }
    public class OzonRole
    {
        public string Name = default! ;
        public List<string> Methods = new List<string>();
    }
}
