using JobNexus.Common.Enum;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Extensions;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Authorization;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyRequestController : ControllerBase
    {
        private readonly ICompanyRequestService _companyRequestService;
        private readonly IAuthorizationService _authorizationService;

        public CompanyRequestController(ICompanyRequestService companyRequestService, IAuthorizationService authorizationService)
        {
            _companyRequestService = companyRequestService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        [ResponseMessage(message: "Fetch request info successfully")]
        public async Task<ActionResult<ApiDataResponse<CompanyRequestDto>>> GetById([FromRoute] int id)
        {
            var request = await _companyRequestService.GetByIdAsync(id);
            if (request == null)
            {
                return new NotFoundObjectResult(new ErrorResponse()
                {
                    Error = "Not Found",
                    Messages = ["Request not found with provided id"]
                });
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, request, Operations.Read);

            if (!authorizationResult.Succeeded)
                return new ForbidResult();

            return Ok(request.ToCompanyRequestDto());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ResponseMessage(message: "Fetch request list successfully")]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<CompanyRequestDto>>>> GetList([FromQuery] CompanyRequestQueryDto companyRequestQueryDto)
        {
            var data = (await _companyRequestService.GetAllAsync(companyRequestQueryDto, User));

            return Ok(data);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: "Create request successfully")]
        public async Task<ActionResult<ApiDataResponse<CompanyRequestDto>>> CreateRequest([FromForm] CreateCompanyRequestDto createCompanyRequestDto)
        {
            var userId = User.GetUserId();

            var companyRequest = await _companyRequestService.CreateRequestAsync(userId!, createCompanyRequestDto);

            if(companyRequest == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Error = "Can not create more requests",
                    Messages = ["Current pending or approved request already exists"]
                });
            }

            return StatusCode(StatusCodes.Status201Created, companyRequest.ToCompanyRequestDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ResponseMessage(message: "Update request status successfully")]
        public async Task<ActionResult<ApiDataResponse<CompanyRequestDto>>> UpdateStatus([FromRoute] int id, [FromBody] UpdateCompanyRequestDto updateCompanyRequestDto)
        {
            var request = await _companyRequestService.UpdateStatusAsync(id, updateCompanyRequestDto);

            if(request == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Error = "Invalid Status Update",
                    Messages = ["Cannot update status to Pending/ Target resource status can no longer be updated"]
                });
            }

            return Ok(request.ToCompanyRequestDto());
        }
    }
}
