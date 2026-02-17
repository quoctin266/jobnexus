using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Extensions;
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
    public class CompanyEmployeeController : ControllerBase
    {
        private readonly ICompanyEmployeeService _companyEmployeeService;

        public CompanyEmployeeController(ICompanyEmployeeService companyEmployeeService)
        {
            _companyEmployeeService = companyEmployeeService;
        }

        [Authorize(Roles = "Admin, Employer")]
        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListEmployee)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<CompanyEmployeeDto>>>> GetList([FromQuery] CompanyEmployeeQueryDto companyEmployeeQueryDto)
        {
            var result = await _companyEmployeeService.GetAll(companyEmployeeQueryDto, User);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin, Employer")]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneEmployee)]
        public async Task<ActionResult<ApiDataResponse<CompanyEmployeeDto>>> GetById([FromRoute] int id)
        {
            var result = await _companyEmployeeService.GetById(id);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToCompanyEmployeeDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: SuccessMessages.CreateEmployee)]
        public async Task<ActionResult<ApiDataResponse<CompanyEmployeeDto>>> Create([FromForm] CreateFormDto createFormDto)
        {
            var userId = User.GetUserId();
            var result = await _companyEmployeeService.Create(createFormDto, userId!);

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

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToCompanyEmployeeDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPatch("deactive/{id}")]
        [ResponseMessage(message: SuccessMessages.DeactivateEmployee)]
        public async Task<ActionResult<ApiDataResponse<CompanyEmployeeDto>>> UpdateToInactive([FromRoute] int id)
        {
            var result = await _companyEmployeeService.UpdateToInactive(id, User);

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

            return Ok(result.Value?.ToCompanyEmployeeDto());
        }
    }
}
