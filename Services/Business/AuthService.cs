using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Dtos.Auth;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;

namespace JobNexus.Services.Business
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ITokenService _tokenService;

        public AuthService(IAccountRepository accountRepository, ITokenService tokenService)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _accountRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
                return ServiceResult<LoginResponseDto>.Failure(StatusCodes.Status401Unauthorized, 
                                                              Error.UnAuthorized, 
                                                              [ErrorMessages.InvalidCredentials]);

            var passwordCheck = await _accountRepository.CheckPasswordAsync(user, loginDto.Password);

            if (!passwordCheck.Succeeded)
                return ServiceResult<LoginResponseDto>.Failure(StatusCodes.Status401Unauthorized, 
                                                              Error.UnAuthorized, 
                                                              [ErrorMessages.InvalidCredentials]);

            var AccessToken = await _tokenService.CreateToken(user, TokenType.AccessToken);
            var RefreshToken = await _tokenService.CreateToken(user, TokenType.RefreshToken);

            var response = new LoginResponseDto(AccessToken, RefreshToken);

            return ServiceResult<LoginResponseDto>.Success(response);
        }

        public async Task<ServiceResult<AppUser>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var createResult = await _accountRepository.CreateUserAsync(user, registerDto.Password);

            if (!createResult.Succeeded)
            {
                var messages = createResult.Errors.Select(e => e.Description).ToList();

                return ServiceResult<AppUser>.Failure(StatusCodes.Status400BadRequest, 
                                                      Error.ValidationFailed, messages);
            }
           
            var roleResult = await _accountRepository.UpdateUserRoleAsync(user, Role.User);
            if (!roleResult.Succeeded)
            {
                return ServiceResult<AppUser>.Failure(StatusCodes.Status500InternalServerError,
                                                      Error.ServerFailure,
                                                      [ErrorMessages.ServerError]);
            }

            return ServiceResult<AppUser>.Success(user);
        }
    }
}
