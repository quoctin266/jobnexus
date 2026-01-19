using JobNexus.Dtos.User;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public UserController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseMessage(message: "Fetch user info successfully")]
        public async Task<ActionResult<DataResponse<UserDto>>> GetById([FromRoute] string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if(user == null)
            {
                return new NotFoundObjectResult(new ErrorResponse()
                {
                    Error = "Not Found",
                    Messages = [ "User not found with provided id" ]
                });
            }

            return Ok(user.ToUserDto());
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        [ResponseMessage(message: "Update user successfully")]
        public async Task<ActionResult<DataResponse<UserDto>>> Update([FromRoute] string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _accountRepository.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorResponse()
                {
                    Error = "Not Found",
                    Messages = ["User not found with provided id"]
                });
            }

            return Ok(user.ToUserDto());
        }
    }
}
