using JobNexus.Common.Enum;
using JobNexus.Dtos.User;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Interfaces.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);

        Task<AppUser> UpdateUserAsync(AppUser user, UpdateUserDto updateUserDto);

        Task<IdentityResult> DeleteAsync(AppUser user);

        Task<IdentityResult> UpdateUserRoleAsync(AppUser user, Role role);

        Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token);

        Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword);

        Task<string> GetUserRoleAsync(AppUser user);

        Task<AppUser?> GetByIdAsync(string id);

        Task<AppUser?> GetByEmailAsync(string email);

        Task<SignInResult> CheckPasswordAsync(AppUser user, string password);

        Task<IdentityResult> InvalidateTokensAsync(AppUser user);

        Task<string> GenerateTokenAsync(AppUser user, TokenPurpose purpose);
    }
}
