using JobNexus.Dtos.Auth;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IAuthService
    {
        Task<ServiceResult<AppUser>> Register(RegisterDto registerDto);

        Task<ServiceResult<LoginResponseDto>> Login(LoginDto loginDto);
    }
}
