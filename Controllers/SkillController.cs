using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Skill;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListSkill)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<SkillDto>>>> GetList([FromQuery] SkillQueryDto skillQueryDto)
        {
            var result = await _skillService.GetAll(skillQueryDto);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ResponseMessage(message: SuccessMessages.CreateSkill)]
        public async Task<ActionResult<ApiDataResponse<SkillDto>>> Create([FromBody] CreateSkillDto createSkillDto)
        {
            var result = await _skillService.Create(createSkillDto);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToSkillDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateSkill)]
        public async Task<ActionResult<ApiDataResponse<SkillDto>>> Update([FromRoute] int id, [FromBody] UpdateSkillDto updateSkillDto)
        {
            var result = await _skillService.Update(id, updateSkillDto);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToSkillDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ResponseMessage(message: SuccessMessages.DeleteSkill)]
        public async Task<ActionResult<ApiDataResponse<VoidType>>> Delete([FromRoute] int id)
        {
            var result = await _skillService.Delete(id);

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
    }
}
