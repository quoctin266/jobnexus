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
using static JobNexus.Helpers.Utils.HelperFunctions;

namespace JobNexus.Services.Business
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDBContext _context;

        private readonly IAccountRepository _accountRepository;

        private readonly ITokenService _tokenService;

        private readonly ITokenRepository _tokenRepository;

        private readonly IEmailService _emailService;

        private readonly FrontendSettings _frontendSettings;

        public AuthService(IAccountRepository accountRepository, ITokenService tokenService,
                           ITokenRepository tokenRepository, IEmailService emailService,
                           ApplicationDBContext context, IOptions<FrontendSettings> options)
        {
            _context = context;
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
                    Purpose = TokenPurpose.LoginSession,
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

            var expiresAt = DateTime.UtcNow.AddMinutes(5);
            var identity = Guid.NewGuid();
            var token = _tokenService.CreateVerifyToken(identity, expiresAt, user.Email, TokenPurpose.EmailVerification);

            // Store the verify token in the database
            await _tokenRepository.CreateAsync(
                new Token
                {
                    TokenIdentity = identity,
                    Purpose = TokenPurpose.EmailVerification,
                    AppUserId = user.Id,
                    ExpiresAt = expiresAt,
                });

            var model = new ConfirmEmailDto 
            { 
                ConfirmationUrl = $"{_frontendSettings.BaseUrl}/{_frontendSettings.VerifyEmailPath}?token={token}"
            };

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Welcome to JobNexus", 
                                                   "EmailVerification.cshtml", model);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Check exception when sending email: ", ex);
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

            var expiresAt = DateTime.UtcNow.AddMinutes(5);
            var identity = Guid.NewGuid();

            var currentToken = await _tokenRepository.GetByUserAndPurposeAsync(user.Id, sendVerificationDto.Purpose);
            if (currentToken is null)
            {
                await _tokenRepository.CreateAsync(
                     new Token
                     {
                         TokenIdentity = identity,
                         Purpose = sendVerificationDto.Purpose,
                         AppUserId = user.Id,
                         ExpiresAt = expiresAt,
                     });
            }
            else
            {
                await _tokenRepository.UpdateAsync(currentToken, identity, expiresAt);
            }

            var token = _tokenService.CreateVerifyToken(identity, expiresAt,
                                                        sendVerificationDto.Email, sendVerificationDto.Purpose);

            var subject = "";
            var templates = "";
            object model = new {};

            if (sendVerificationDto.Purpose == TokenPurpose.EmailVerification)
            {
                subject = "Welcome to JobNexus";
                templates = "EmailVerification.cshtml";
                model = new ConfirmEmailDto
                {
                    ConfirmationUrl = $"{_frontendSettings.BaseUrl}/{_frontendSettings.VerifyEmailPath}?token={token}"
                };
            }

            if(sendVerificationDto.Purpose == TokenPurpose.PasswordReset)
            {
                subject = "Reset your password";
                templates = "PasswordReset.cshtml";
                model = new PasswordResetDto
                {
                    ResetUrl = $"{_frontendSettings.BaseUrl}/{_frontendSettings.PasswordResetPath}?token={token}",
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
                                              [ErrorMessages.ServerError]);
            }

            return ServiceResult<VoidType>.Success(new VoidType());
        }

        public async Task<ServiceResult<AppUser>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            var principal = _tokenService.ValidateToken(verifyEmailDto.Token);

            if (principal is null)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var identityClaim = principal.GetTokenIdentity();
            var email = principal.GetTokenEmail();
            var purpose = principal.GetTokenPurpose();
            
            if (!Guid.TryParse(identityClaim, out var identity))
                return ServiceResult<AppUser>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);
           
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(purpose) || 
                purpose != TokenPurpose.EmailVerification.ToString())
                return ServiceResult<AppUser>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);
           
            var token = await _tokenRepository.GetByIdentityAsync(identity);
            if (token is null || token.ExpiresAt <= DateTimeOffset.UtcNow || 
                token.Purpose != TokenPurpose.EmailVerification)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            var user = await _accountRepository.GetByIdAsync(token.AppUserId);
            if (user is null)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.UserNotFound]);

            if(user.Email != email)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status401Unauthorized,
                                                              Error.UnAuthorized,
                                                              [ErrorMessages.InvalidToken]);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _tokenRepository.DeleteAsync(token);
                await _accountRepository.ConfirmEmailAsync(user);
                
                await transaction.CommitAsync();

                return ServiceResult<AppUser>.Success(user);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                await transaction.RollbackAsync();
            }

            return ServiceResult<AppUser>.Failure(StatusCodes.Status500InternalServerError,
                                                  Error.ServerFailure,
                                                  [ErrorMessages.ServerError]);
        }
    }
}
