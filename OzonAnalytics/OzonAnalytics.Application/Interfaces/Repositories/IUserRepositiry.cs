using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Repositories
{
    public interface IUserRepositiry
    {
        public Task<bool> IsEmailTakenAsync(string email);
    }
}
