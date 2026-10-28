using Microsoft.AspNetCore.Mvc;
using OzonAnalytics.Application.Interfaces.Auth;
using OzonAnalytics.Application.ProjectDtos.UserDtos;

namespace OzonAnalytics.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(UserRegistrationDto userRegistrationDto)
        {
            var reg = await _authService.RegistrationAsync(userRegistrationDto);
            return Ok(reg);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Authorization(UserAuthDto userAuthDto, CancellationToken ct)
        {
            var auth = await _authService.AuthorizationAsync(userAuthDto, ct);
            return Ok(new { auth.AccessToken, auth.RefreshToken });
        }
    }
}
