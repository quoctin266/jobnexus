using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.User;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Authorization;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneUser)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> GetById([FromRoute] string id)
        {
            var result = await _userService.GetById(id);

            if(!result.IsSuccess)
            {
                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(new ErrorResponse(result.Error, result.Messages)),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error, result.Messages))
                };
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, result.Value, Operations.Read);

            if (!authorizationResult.Succeeded)
                return Forbid();

            return Ok(result.Value?.ToUserDto());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateUser)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> Update([FromRoute] string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.Update(id, updateUserDto, User);

            if(!result.IsSuccess)
            {
                return result.StatusCode switch
                {
                    StatusCodes.Status403Forbidden => Forbid(),
                    StatusCodes.Status404NotFound => NotFound(new ErrorResponse(result.Error, result.Messages)),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error, result.Messages))
                };
            }

            return Ok(result.Value?.ToUserDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ResponseMessage(message: SuccessMessages.DeleteUser)]
        public async Task<ActionResult<ApiDataResponse<UserDto>>> Delete([FromRoute] string id)
        {
            var result = await _userService.Delete(id);

            if(!result.IsSuccess)
            {
                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(new ErrorResponse(result.Error, result.Messages)),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error, result.Messages))
                };
            }

            return Ok(null);
        }
    }
}
