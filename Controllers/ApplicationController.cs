using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Application;
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
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListApplication)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<ApplicationDto>>>> GetList([FromQuery] ApplicationQueryDto applicationQueryDto)
        {
            var result = await _applicationService.GetAll(applicationQueryDto, User);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Employer, User")]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneApplication)]
        public async Task<ActionResult<ApiDataResponse<ApplicationDto>>> GetById([FromRoute] int id)
        {
            var result = await _applicationService.FindById(id);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToApplicationDto());
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ResponseMessage(message: SuccessMessages.CreateApplication)]
        public async Task<ActionResult<ApiDataResponse<ApplicationDto>>> Create([FromBody] CreateApplicationDto createApplicationDto)
        {
            var result = await _applicationService.Create(createApplicationDto, User);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToApplicationDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPatch("status/{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateApplicationStatus)]
        public async Task<ActionResult<ApiDataResponse<ApplicationDto>>> UpdateStatus([FromRoute] int id, [FromBody] UpdateApplicationStatusDto updateApplicationStatusDto)
        {
            var result = await _applicationService.UpdateStatus(id, updateApplicationStatusDto, User);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    StatusCodes.Status403Forbidden => Forbid(),
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToApplicationDto());
        }
    }
}
