using Microsoft.AspNetCore.Identity;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.ResponceDto;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Services.UserService
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepositiry _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(UserManager<ApplicationUser> userManager, IUserRepositiry userRepositiry, IJwtTokenGenerator jwtTokenGenerator) 
        { 
            _userManager = userManager;
            _userRepository = userRepositiry;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string?> RegistrationAsync(UserRegistrationDto userRegistrationDto)
        {
            if (userRegistrationDto is null) { throw new Exception(); } // notfound??
            if (await _userManager.FindByEmailAsync(userRegistrationDto.Email) is not null) throw new Exception(); // conflict
            var user = new ApplicationUser { Email = userRegistrationDto.Email};
            await _userManager.CreateAsync(user, userRegistrationDto.Password);

            // add authoriz 
            return user.Email;
        }
        public async Task<AuthResponceDto> AuthorizationAsync(UserAuthDto userAuthDto)
        {
            var currentUser = await _userManager.FindByEmailAsync(userAuthDto.email);
            if (currentUser is null || !await _userManager.CheckPasswordAsync(currentUser, userAuthDto.password))
            {
                //logger
                throw new Exception(); // unauthorizedexc
            }
            // check emailconfirmed

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(currentUser);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var hashRefreshToken = _jwtTokenGenerator.HashRefreshToken(refreshToken);

            var tokenEntity = new RefreshToken(currentUser.Id, hashRefreshToken);

            await _userRepository.AddAsync(tokenEntity);

            await _userRepository.SaveChangesAsync();

            return new AuthResponceDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }
    }
}
