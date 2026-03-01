using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.Auth;
using JobNexus.Dtos.Email;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.Extensions.Options;
using System.Net;
using static JobNexus.Helpers.Utils.HelperFunctions;

namespace JobNexus.Services.Business
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ITokenService _tokenService;

        private readonly ITokenRepository _tokenRepository;

        private readonly IEmailService _emailService;

        private readonly FrontendSettings _frontendSettings;

        public AuthService(IAccountRepository accountRepository, ITokenService tokenService,
                           ITokenRepository tokenRepository, IEmailService emailService,
                           IOptions<FrontendSettings> options)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _frontendSettings = options.Value;
        }
        public async Task<ServiceResult<TokenResponseDto>> Login(LoginDto loginDto, HttpResponse response)
        {
            var user = await _accountRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidCredentials]);

            if(!user.EmailConfirmed)
                return ServiceResult<TokenResponseDto>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.EmailNotVerified]);

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

            var token = await _accountRepository.GenerateTokenAsync(user, TokenPurpose.EmailVerification);

            var model = new ConfirmEmailDto 
            { 
                ConfirmationUrl = $"{_frontendSettings.BaseUrl}/{_frontendSettings.VerifyEmailPath}?email={user.Email}&token={token}"
            };

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Welcome to JobNexus", 
                                                   "EmailVerification.cshtml", model);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Check exception when sending email: ", ex.Message);
            }

            return ServiceResult<AppUser>.Success(user);
        }

        public async Task<ServiceResult<VoidType>> SendVerification(SendVerificationDto sendVerificationDto)
        {
            var user = await _accountRepository.GetByEmailAsync(sendVerificationDto.Email);
            if(user is null)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status404NotFound,
                                                       Error.NotFound,
                                                       [ErrorMessages.EmailNotFound]);

            if (user.EmailConfirmed && sendVerificationDto.Purpose == TokenPurpose.EmailVerification)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status409Conflict,
                                          Error.ResourceConflict,
                                          [ErrorMessages.EmailAlreadyVerified]);

            if(!user.EmailConfirmed && sendVerificationDto.Purpose == TokenPurpose.PasswordReset)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status409Conflict,
                                          Error.ResourceConflict,
                                          [ErrorMessages.EmailNotVerified]);

            var result = await _accountRepository.InvalidateTokensAsync(user);
            if(!result.Succeeded)
            {
                return ServiceResult<VoidType>.Failure(StatusCodes.Status500InternalServerError,
                                              Error.ServerFailure,
                                              [.. result.Errors.Select(e => e.Description)]);
            }

            var token = await _accountRepository.GenerateTokenAsync(user, sendVerificationDto.Purpose);

            var subject = "";
            var templates = "";
            object model = new {};

            if (sendVerificationDto.Purpose == TokenPurpose.EmailVerification)
            {
                subject = "Welcome to JobNexus";
                templates = "EmailVerification.cshtml";
                model = new ConfirmEmailDto
                {
                    ConfirmationUrl = $"{_frontendSettings.BaseUrl}/{_frontendSettings.VerifyEmailPath}?" +
                    $"email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(token)}"
                };
            }

            if(sendVerificationDto.Purpose == TokenPurpose.PasswordReset)
            {
                subject = "Reset your password";
                templates = "PasswordReset.cshtml";
                model = new PasswordResetDto
                {
                    ResetUrl = $"{_frontendSettings.BaseUrl}/{_frontendSettings.PasswordResetPath}?" +
                    $"email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(token)}",
                    Username = user.UserName ?? ""
                };
            }

            try
            {
                await _emailService.SendEmailAsync(sendVerificationDto.Email, subject,
                                                   templates, model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check exception when sending email: ", ex);

                return ServiceResult<VoidType>.Failure(StatusCodes.Status500InternalServerError,
                                              Error.ServerFailure,
                                              [ex.Message]);
            }

            return ServiceResult<VoidType>.Success(new VoidType());
        }

        public async Task<ServiceResult<AppUser>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            var decodedEmail = WebUtility.UrlDecode(verifyEmailDto.Email);

            var user = await _accountRepository.GetByEmailAsync(decodedEmail);
            if (user is null)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.EmailNotFound]);

            if(user.EmailConfirmed)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status409Conflict,
                                                              Error.ResourceConflict,
                                                              [ErrorMessages.EmailAlreadyVerified]);

            var decodedToken = WebUtility.UrlDecode(verifyEmailDto.Token);

            var confirmResult = await _accountRepository.ConfirmEmailAsync(user, decodedToken);
            if (!confirmResult.Succeeded)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status400BadRequest,
                                                      Error.ValidationFailed,
                                                      [.. confirmResult.Errors.Select(e => e.Description)]);

            var result = await _accountRepository.InvalidateTokensAsync(user);
            if (!result.Succeeded)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status500InternalServerError,
                                                  Error.ServerFailure,
                                                  [.. result.Errors.Select(e => e.Description)]);

            return ServiceResult<AppUser>.Success(user);
        }

        public async Task<ServiceResult<AppUser>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var decodedEmail = WebUtility.UrlDecode(resetPasswordDto.Email);

            var user = await _accountRepository.GetByEmailAsync(decodedEmail);
            if (user is null)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.EmailNotFound]);

            if(!user.EmailConfirmed)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status409Conflict,
                                                              Error.ResourceConflict,
                                                              [ErrorMessages.EmailNotVerified]);
            
            //var passwordCheck = await _accountRepository.CheckPasswordAsync(user, resetPasswordDto.NewPassword);

            var decodedToken = WebUtility.UrlDecode(resetPasswordDto.Token);

            var resetResult = await _accountRepository.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (!resetResult.Succeeded)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status400BadRequest,
                                                      Error.ValidationFailed,
                                                      [.. resetResult.Errors.Select(e => e.Description)]);

            var result = await _accountRepository.InvalidateTokensAsync(user);
            if (!result.Succeeded)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status500InternalServerError,
                                                  Error.ServerFailure,
                                                  [.. result.Errors.Select(e => e.Description)]);

            return ServiceResult<AppUser>.Success(user);
        }
    }
}
