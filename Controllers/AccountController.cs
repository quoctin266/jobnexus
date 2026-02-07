using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Dtos.Auth;
using JobNexus.Dtos.User;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static JobNexus.Helpers.Utils.MyFunctions;

namespace JobNexus.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ITokenService _tokenService;

        public AccountController(IAccountRepository accountRepository, ITokenService tokenService)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ResponseMessage(message: SuccessMessages.RegisterSuccess)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var createdUser = await _accountRepository.CreateUserAsync(user, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var roleResult = await _accountRepository.UpdateUserRoleAsync(user, Role.User);
                if (roleResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created, user.ToUserDto());
                }

                return StatusCode(500, roleResult.Errors);
            }

            return new BadRequestObjectResult(ToErrorResponse(createdUser.Errors));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ResponseMessage(message: SuccessMessages.LoginSuccess)]
        public async Task<ActionResult<ApiDataResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var unauthorizedResponse = new ErrorResponse(Error.UnAuthorized, [ErrorMessages.InvalidCredentials]);

            var user = await _accountRepository.GetByEmailAsync(loginDto.Email);

            if (user == null) return new UnauthorizedObjectResult(unauthorizedResponse);

            var result = await _accountRepository.CheckPasswordAsync(user, loginDto.Password);

            if (!result.Succeeded) return new UnauthorizedObjectResult(unauthorizedResponse);

            var response = new LoginResponseDto
            {
                AccessToken = await _tokenService.CreateToken(user, TokenType.AccessToken),
                RefreshToken = await _tokenService.CreateToken(user, TokenType.RefreshToken),
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
