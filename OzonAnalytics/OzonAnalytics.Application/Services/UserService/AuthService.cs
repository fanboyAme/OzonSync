using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponceDto;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Application.Exceptions;
using OzonAnalytics.Application.Interfaces.Auth;

namespace OzonAnalytics.Application.Services.UserService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;  
        public AuthService(UserManager<ApplicationUser> userManager, IUserRepository userRepositiry, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger) 
        { 
            _userManager = userManager;
            _userRepository = userRepositiry;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<string?> RegistrationAsync(UserRegistrationDto userRegistrationDto)
        {
            if (userRegistrationDto is null) { throw new NotFoundException("Пользователь не найден"); }
            if (await _userManager.FindByEmailAsync(userRegistrationDto.Email) is not null) 
            {
                _logger.LogError("Ошибка создания пользователя. Почта {UserEmail} уже занят",
                    userRegistrationDto.Email
                    );
                throw new ConflictException("Почта уже занята"); 
            }
            var user = new ApplicationUser { UserName = userRegistrationDto.Email, Email = userRegistrationDto.Email};
            var result = await _userManager.CreateAsync(user, userRegistrationDto.Password);
            if(!result.Succeeded)
            {
                _logger.LogError("Ошибка создания пользователя {UserId}", user.Id);
                throw new UserRegistrationException($"Ошибка создания пользователя {user.Id}");
            }

            // add authoriz if start realiz email verif
            _logger.LogInformation("Пользователь {UserId} успешно создан", user.Id);
            return user.Email;
        }
        public async Task<AuthResponceDto> AuthorizationAsync(UserAuthDto userAuthDto)
        {
            var currentUser = await _userManager.FindByEmailAsync(userAuthDto.email);
            if (currentUser is null || !await _userManager.CheckPasswordAsync(currentUser, userAuthDto.password))
            {
                _logger.LogError("Неверна почта или пароль");
                throw new UnauthorizedException("Неверна почта или пароль");
            }
            // check emailconfirmed if add realiz email verify

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(currentUser);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var hashRefreshToken = _jwtTokenGenerator.HashRefreshToken(refreshToken);

            var tokenEntity = new RefreshToken(currentUser.Id, hashRefreshToken);

            await _userRepository.AddAsync(tokenEntity);

            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Пользователь {UserId} успешно прошел авторизацию", currentUser.Id);

            return new AuthResponceDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }
    }
}
