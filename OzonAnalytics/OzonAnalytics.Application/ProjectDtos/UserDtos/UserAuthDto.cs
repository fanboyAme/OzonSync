using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.ProjectDtos.UserDtos
{
    public record class UserAuthDto(string email, string password);
}
