using JobNexus.Dtos.User;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IUserService
    {
        Task<ServiceResult<AppUser>> GetById(string id);

        Task<ServiceResult<AppUser>> Update(string id, UpdateUserDto updateUserDto, ClaimsPrincipal userClaims);

        Task<ServiceResult<VoidType>> Delete(string id);
    }
}
