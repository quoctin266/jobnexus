using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.User;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
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

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneUser)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> GetById([FromRoute] string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if(user == null)
            {
                return new NotFoundObjectResult(new ErrorResponse(Error.NotFound, [ErrorMessages.UserNotFound]));
            }

            return Ok(user.ToUserDto());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateUser)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> Update([FromRoute] string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _accountRepository.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorResponse(Error.NotFound, [ErrorMessages.UserNotFound]));
            }

            return Ok(user.ToUserDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ResponseMessage(message: SuccessMessages.DeleteUser)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> Delete([FromRoute] string id)
        {
            var user = await _accountRepository.DeleteAsync(id);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorResponse(Error.NotFound, [ErrorMessages.UserNotFound]));
            }

            return Ok(user.ToUserDto());
        }
    }
}
