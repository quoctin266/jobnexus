using JobNexus.Common.Enum;
using JobNexus.Dtos.User;
using JobNexus.Interfaces.Repository;
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

        public async Task<IdentityResult> UpdateUserRoleAsync(AppUser user, Role role)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded) return removeResult;
            }

            return await _userManager.AddToRoleAsync(user, role.ToString());
        }

        public async Task<string> GetUserRoleAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return roles[0];
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

        public async Task<AppUser> UpdateUserAsync(AppUser user, UpdateUserDto updateUserDto)
        {
            user.UserName = updateUserDto.Username;
            user.DateOfBirth = DateTime.Parse(updateUserDto.DateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            user.Gender = updateUserDto.Gender;
            user.Address = updateUserDto.Address;
            user.PhoneNumber = updateUserDto.PhoneNumber;

            await _userManager.UpdateAsync(user);

            return user;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> InvalidateTokensAsync(AppUser user)
        {
            return await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<string> GenerateTokenAsync(AppUser user, TokenPurpose purpose)
        {
            if(purpose == TokenPurpose.EmailVerification)
            {
                return await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}
