using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Auth;
using JobNexus.Dtos.User;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
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
        public async Task<ActionResult<ApiDataResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);

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
    }
}
