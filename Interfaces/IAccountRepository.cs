using JobNexus.Common.Enum;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);

        Task<IdentityResult> AddRoleToUserAsync(AppUser user, Role role);

        Task<AppUser?> GetByIdAsync(string id);

    }
}
