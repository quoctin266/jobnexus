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
        [HttpGet("{id}")]
        [ResponseMessage(message: "Fetch employee info successfully")]
        public async Task<ActionResult<ApiDataResponse<CompanyEmployeeDto>>> GetById([FromRoute] int id)
        {
            var employee = await _companyEmployeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return new NotFoundObjectResult(new ErrorResponse()
                {
                    Error = "Not Found",
                    Messages = ["Employee not found with provided id"]
                });
            }

            return Ok(employee.ToCompanyEmployeeDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: "Create company employee successfully")]
        public async Task<ActionResult<ApiDataResponse<CompanyEmployeeDto>>> Create([FromForm] CreateFormDto createFormDto)
        {
            var userId = User.GetUserId();
            var companyEmployee = await _companyEmployeeService.CreateAsync(createFormDto, userId!);

            if (companyEmployee == null)
            {
                return new ForbidResult();
            }

            return StatusCode(StatusCodes.Status201Created, companyEmployee.ToCompanyEmployeeDto());
        }
    }
}
