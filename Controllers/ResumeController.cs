using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Resume;
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
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListResume)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<ResumeDto>>>> GetList([FromQuery] ResumeQueryDto resumeQueryDto)
        {
            var result = await _resumeService.GetAll(resumeQueryDto, User);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneResume)]
        public async Task<ActionResult<ApiDataResponse<ResumeDto>>> GetById([FromRoute] int id)
        {
            var result = await _resumeService.FindById(id, User);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status403Forbidden => Forbid(),
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToResumeDto());
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: SuccessMessages.CreateResume)]
        public async Task<ActionResult<ApiDataResponse<ResumeDto>>> Create([FromForm] CreateResumeDto createResumeDto)
        {
            var result = await _resumeService.Create(createResumeDto, User);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToResumeDto());
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: SuccessMessages.UpdateResume)]
        public async Task<ActionResult<ApiDataResponse<ResumeDto>>> Update([FromRoute] int id, [FromForm] UpdateResumeDto updateResumeDto)
        {
            var result = await _resumeService.Update(id, updateResumeDto, User);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status403Forbidden => Forbid(),
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToResumeDto());
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        [ResponseMessage(message: SuccessMessages.DeleteResume)]
        public async Task<ActionResult<ApiDataResponse<VoidType>>> Delete([FromRoute] int id)
        {
            var result = await _resumeService.Delete(id, User);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    StatusCodes.Status403Forbidden => Forbid(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(null);
        }
    }
}
