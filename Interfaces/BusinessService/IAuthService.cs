using JobNexus.Dtos.Auth;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IAuthService
    {
        Task<ServiceResult<AppUser>> Register(RegisterDto registerDto);

        Task<ServiceResult<TokenResponseDto>> Login(LoginDto loginDto, HttpResponse response);

        Task<ServiceResult<TokenResponseDto>> Refresh(HttpRequest request, HttpResponse response);

        Task<ServiceResult<VoidType>> Logout(HttpRequest request, HttpResponse response);

        Task<ServiceResult<AppUser>> VerifyEmail(VerifyEmailDto verifyEmailDto);

        Task<ServiceResult<AppUser>> ResetPassword(ResetPasswordDto resetPasswordDto);

        Task<ServiceResult<VoidType>> SendVerification(SendVerificationDto sendVerificationDto);
    }
}
