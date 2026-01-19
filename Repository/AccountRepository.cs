using JobNexus.Common.Enum;
using JobNexus.Dtos.User;
using JobNexus.Interfaces;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddRoleToUserAsync(AppUser user, Role role)
        {
            return await _userManager.AddToRoleAsync(user, role.ToString());
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<AppUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<AppUser?> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                user.UserName = updateUserDto.Username;
                user.Age = updateUserDto.Age;
                user.Address = updateUserDto.Address;
                user.PhoneNumber = updateUserDto.PhoneNumber;

                await _userManager.UpdateAsync(user);
            }

            return user;
        }
    }
}
