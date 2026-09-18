using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace OzonAnalytics.Infrastructure.Auth
{
    public class FindCurrentUser: IFindCurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FindCurrentUser> _logger;

        public FindCurrentUser(ILogger<FindCurrentUser> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid FindCurrentUserId()
        {
            try
            {
                if (_httpContextAccessor.HttpContext is null) throw new UnauthorizedException("Пользователь не авторизован");
                var findFirst = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("Пользователь не авторизован"); ;
                return Guid.Parse(findFirst.Value);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Неверный формат идентификатора пользователя");
                throw new UnauthorizedException("Неверный формат идентификатора пользователя");
            }
        }
    }
}
