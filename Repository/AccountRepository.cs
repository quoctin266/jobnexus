using JobNexus.Common.Enum;
using JobNexus.Dtos.User;
using JobNexus.Interfaces;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

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

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SignInResult> CheckPasswordAsync(AppUser user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<AppUser?> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                user.UserName = updateUserDto.Username;
                user.DateOfBirth = DateTime.Parse(updateUserDto.DateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                user.Gender = updateUserDto.Gender;
                user.Address = updateUserDto.Address;
                user.PhoneNumber = updateUserDto.PhoneNumber;

                await _userManager.UpdateAsync(user);
            }

            return user;
        }

        public async Task<AppUser?> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) {
                    Console.WriteLine("Failed to delete user:", result.Errors);

                    return null;
                };
            }

            return user;
        }


    }
}
