using JobNexus.Common.Enum;
using JobNexus.Dtos.User;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Interfaces.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);

        Task<IdentityResult> UpdateUserRoleAsync(AppUser user, Role role);

        Task<AppUser?> UpdateUserAsync(string id, UpdateUserDto updateUserDto);

        Task<AppUser?> GetByIdAsync(string id);

        Task<AppUser?> DeleteAsync(string id);

        Task<AppUser?> GetByEmailAsync(string email);

        Task<SignInResult> CheckPasswordAsync(AppUser user, string password);
    }
}
