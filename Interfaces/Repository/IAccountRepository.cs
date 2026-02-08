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

        Task<AppUser?> GetByIdAsync(string id);

        Task<AppUser?> GetByEmailAsync(string email);

        Task<SignInResult> CheckPasswordAsync(AppUser user, string password);
    }
}
