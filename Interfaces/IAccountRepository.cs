using JobNexus.Common.Enum;
using JobNexus.Dtos.User;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);

        Task<IdentityResult> AddRoleToUserAsync(AppUser user, Role role);

        Task<AppUser?> UpdateUserAsync(string id, UpdateUserDto updateUserDto);

        Task<AppUser?> GetByIdAsync(string id);

    }
}
