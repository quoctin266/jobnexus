using JobNexus.Common.Enum;
using JobNexus.Dtos.Auth;
using JobNexus.Interfaces;
using JobNexus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        //[ResponseMessage(message: "Register successfully")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var createdUser = await _accountRepository.CreateUserAsync(user, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var roleResult = await _accountRepository.AddRoleToUserAsync(user, Role.User);
                if (roleResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created, new
                    {
                        username = user.UserName,
                        email = user.Email,
                    });
                }

                return StatusCode(500, roleResult.Errors);
            }

            return new BadRequestObjectResult(createdUser.Errors);
        }

    }
}
