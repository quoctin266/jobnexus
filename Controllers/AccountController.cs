using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Auth;
using JobNexus.Dtos.User;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Mappers;
using JobNexus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NpgsqlTypes;
using System.Text.Json;

namespace JobNexus.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly LinkGenerator _linkGenerator;

        public AccountController(IAuthService authService, SignInManager<AppUser> signInManager, LinkGenerator linkGenerator)
        {
            _authService = authService;
            _signInManager = signInManager;
            _linkGenerator = linkGenerator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ResponseMessage(message: SuccessMessages.RegisterSuccess)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.Register(registerDto);

            if(!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToUserDto());
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ResponseMessage(message: SuccessMessages.LoginSuccess)]
        public async Task<ActionResult<ApiDataResponse<TokenResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto, Response);

            if(!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => Unauthorized(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        [AllowAnonymous]
        [HttpGet("google/login")]
        public ActionResult GoogleLogin([FromQuery] string returnUrl)
        {
            var redirectUrl = _linkGenerator.GetUriByName(HttpContext, "GoogleCallback", new { returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return Challenge(properties, "Google");
        }

        [AllowAnonymous]
        [HttpGet("google/callback", Name = "GoogleCallback")]
        public async Task<ActionResult> GoogleCallback([FromQuery] string returnUrl)
        {
            var result = await _authService.GoogleLogin(Response);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => Unauthorized(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            var accessToken = result.Value?.AccessToken;

            return Redirect(returnUrl + $"?accessToken={accessToken}");
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [ResponseMessage(message: SuccessMessages.RefreshSuccess)]
        public async Task<ActionResult<ApiDataResponse<TokenResponseDto>>> Refresh()
        {
            var result = await _authService.Refresh(Request, Response);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => Unauthorized(response),
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        [HttpPost("logout")]
        [ResponseMessage(message: SuccessMessages.LogoutSuccess)]
        public async Task<ActionResult<ApiDataResponse<VoidType>>> Logout()
        {
            var result = await _authService.Logout(Request, Response);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => Unauthorized(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(null);
        }

        [AllowAnonymous]
        [HttpPut("verify-email")]
        [ResponseMessage(message: SuccessMessages.VerifyEmailSuccess)]
        public async Task<ActionResult<ApiDataResponse<UserSummaryDto>>> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            var result = await _authService.VerifyEmail(verifyEmailDto);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    StatusCodes.Status404NotFound => NotFound(response),
                    StatusCodes.Status409Conflict => Conflict(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToUserSummaryDto());
        }

        [AllowAnonymous]
        [HttpPost("send-email")]
        [ResponseMessage(message: SuccessMessages.SendEmailSuccess)]
        public async Task<ActionResult<ApiDataResponse<VoidType>>> SendEmail([FromBody] SendVerificationDto sendVerificationDto)
        {
            var result = await _authService.SendVerification(sendVerificationDto);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    StatusCodes.Status409Conflict => Conflict(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(null);
        }

        [AllowAnonymous]
        [HttpPut("reset-password")]
        [ResponseMessage(message: SuccessMessages.ResetPasswordSuccess)]
        public async Task<ActionResult<ApiDataResponse<VoidType>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPassword(resetPasswordDto);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    StatusCodes.Status404NotFound => NotFound(response),
                    StatusCodes.Status409Conflict => Conflict(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToUserSummaryDto());
        }
    }
}
