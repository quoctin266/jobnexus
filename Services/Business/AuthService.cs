using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Dtos.Auth;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;

using static JobNexus.Helpers.Utils.HelperFunctions;

namespace JobNexus.Services.Business
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ITokenService _tokenService;

        private readonly ITokenRepository _tokenRepository;

        public AuthService(IAccountRepository accountRepository, ITokenService tokenService,
                           ITokenRepository tokenRepository)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
            _tokenRepository = tokenRepository;
        }
        public async Task<ServiceResult<TokenResponseDto>> Login(LoginDto loginDto, HttpResponse response)
        {
            var user = await _accountRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidCredentials]);

            var passwordCheck = await _accountRepository.CheckPasswordAsync(user, loginDto.Password);

            if (!passwordCheck.Succeeded)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidCredentials]);

            var expiresAt = DateTime.UtcNow.AddDays(7);
            var identity = Guid.NewGuid();
            var refreshToken = _tokenService.CreateRefreshToken(identity, expiresAt);

            // Store the refresh token in the database
            await _tokenRepository.CreateAsync(
                new Token
                {
                    TokenIdentity = identity,
                    AppUserId = user.Id,
                    ExpiresAt = expiresAt,
                });

            var AccessToken = await _tokenService.CreateAccessToken(user);

            SetRefreshTokenCookie(response, refreshToken, expiresAt.AddHours(-1));

            return ServiceResult<TokenResponseDto>.Success(new TokenResponseDto(AccessToken));
        }

        public async Task<ServiceResult<VoidType>> Logout(HttpRequest request, HttpResponse response)
        {
            if (!request.Cookies.TryGetValue(TokenType.RefreshToken.ToString(), out var refreshToken) ||
               string.IsNullOrEmpty(refreshToken))
                return ServiceResult<VoidType>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var principal = _tokenService.ValidateToken(refreshToken);
            if (principal is null)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var tokenIdentityClaim = principal.GetTokenIdentity();
            if (!Guid.TryParse(tokenIdentityClaim, out var tokenIdentity))
                return ServiceResult<VoidType>.Failure(StatusCodes.Status500InternalServerError,
                                                              Error.ServerFailure,
                                                              [ErrorMessages.ServerError]);

            var token = await _tokenRepository.GetByIdentityAsync(tokenIdentity);
            if (token is null || token.ExpiresAt <= DateTimeOffset.UtcNow)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            await _tokenRepository.DeleteAsync(token);

            DeleteRefreshTokenCookie(response);

            return ServiceResult<VoidType>.Success(new VoidType());
        }

        public async Task<ServiceResult<TokenResponseDto>> Refresh(HttpRequest request, HttpResponse response)
        {
            if(!request.Cookies.TryGetValue(TokenType.RefreshToken.ToString(), out var refreshToken) ||
                string.IsNullOrEmpty(refreshToken))
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var principal = _tokenService.ValidateToken(refreshToken);
            if (principal is null)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var tokenIdentityClaim = principal.GetTokenIdentity();

            if (!Guid.TryParse(tokenIdentityClaim, out var tokenIdentity))
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var token = await _tokenRepository.GetByIdentityAsync(tokenIdentity);
            if (token is null || token.ExpiresAt <= DateTimeOffset.UtcNow)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var user = await _accountRepository.GetByIdAsync(token.AppUserId);
            if (user is null)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.UserNotFound]);

            // Issue new access token and rotate refresh token
            var newAccessToken = await _tokenService.CreateAccessToken(user);
            var newIdentity = Guid.NewGuid();
            var newExpiresAt = DateTime.UtcNow.AddDays(7);
            var newRefreshToken = _tokenService.CreateRefreshToken(newIdentity, newExpiresAt);

            // Persist new refresh token identity
            await _tokenRepository.UpdateAsync(token, newIdentity, newExpiresAt);

            SetRefreshTokenCookie(response, newRefreshToken, newExpiresAt.AddHours(-1));

            return ServiceResult<TokenResponseDto>.Success(new TokenResponseDto(newAccessToken));
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
