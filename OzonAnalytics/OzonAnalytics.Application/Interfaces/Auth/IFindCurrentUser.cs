using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Auth
{
    public interface IFindCurrentUser
    {
        public Guid FindCurrentUserId();
    }
}
