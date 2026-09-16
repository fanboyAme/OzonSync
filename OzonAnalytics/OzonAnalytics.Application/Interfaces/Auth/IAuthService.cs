using OzonAnalytics.Application.ProjectDtos.ResponceDto;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<string?> RegistrationAsync(UserRegistrationDto userRegistrationDto);
        public Task<AuthResponceDto> AuthorizationAsync(UserAuthDto userAuthDto);
    }
}
