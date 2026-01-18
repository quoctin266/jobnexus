using JobNexus.Dtos.User;
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
        public async Task<ActionResult<UserDto>> GetById([FromRoute] string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if(user == null)
            {
                return NotFound("User not found with provided id");
            }

            return Ok(user.ToUserDto());
        }
    }
}
