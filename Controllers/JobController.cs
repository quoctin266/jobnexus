using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Job;
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
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListJob)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<JobDto>>>> GetList([FromQuery] JobQueryDto jobQueryDto)
        {
            var result = await _jobService.GetAll(jobQueryDto);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneJob)]
        public async Task<ActionResult<ApiDataResponse<JobDto>>> GetById([FromRoute] int id)
        {
            var result = await _jobService.FindById(id);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToJobDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPost]
        [ResponseMessage(message: SuccessMessages.CreateJob)]
        public async Task<ActionResult<ApiDataResponse<JobDto>>> Create([FromBody] CreateJobDto createJobDto)
        {
            var result = await _jobService.Create(createJobDto, User);

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

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToJobDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPatch("status/{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateJobStatus)]
        public async Task<ActionResult<ApiDataResponse<JobDto>>> UpdateStatus([FromRoute] int id, [FromBody] UpdateJobStatusDto updateJobStatusDto)
        {
            var result = await _jobService.UpdateStatus(id, updateJobStatusDto, User);

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

            return Ok(result.Value?.ToJobDto());
        }
    }
}
