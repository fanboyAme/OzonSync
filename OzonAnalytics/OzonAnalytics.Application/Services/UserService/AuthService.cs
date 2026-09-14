using Microsoft.AspNetCore.Identity;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.ProjectDtos.UserDtos;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Application.Services.UserService
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepositiry _userRepository;
        public AuthService(UserManager<ApplicationUser> userManager, IUserRepositiry userRepositiry) 
        { 
            _userManager = userManager;
            _userRepository = userRepositiry;
        }

        public async Task<string?> RegistrationAsync(UserRegistrationDto userRegistrationDto)
        {
            if (userRegistrationDto is null) { throw new Exception(); } // notfound??
            if(await _userRepository.IsEmailTakenAsync(userRegistrationDto.Email)) throw new Exception(); // conflict

            var user = new ApplicationUser { Email = userRegistrationDto.Email};
            await _userManager.CreateAsync(user, userRegistrationDto.Password);

            // add authoriz 
            return user.Email;
        }
        public async Task<AuthResponceDto> AuthorizationAsync (UserauthDto userauthDto)
        {

        }
    }
}
