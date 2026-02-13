using JobNexus.Common.Constant.Messages;
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
        [ResponseMessage(message: SuccessMessages.FetchOneCompanyRequest)]
        public async Task<ActionResult<ApiDataResponse<CompanyRequestDto>>> GetById([FromRoute] int id)
        {
            var result = await _companyRequestService.GetById(id);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, result.Value, Operations.Read);

            if (!authorizationResult.Succeeded)
                return Forbid();

            return Ok(result.Value?.ToCompanyRequestDto());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListCompanyRequest)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<CompanyRequestDto>>>> GetList([FromQuery] CompanyRequestQueryDto companyRequestQueryDto)
        {
            var result = await _companyRequestService.GetAll(companyRequestQueryDto, User);

            return Ok(result.Value);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: SuccessMessages.CreateCompanyRequest)]
        public async Task<ActionResult<ApiDataResponse<CompanyRequestDto>>> CreateRequest([FromForm] CreateCompanyRequestDto createCompanyRequestDto)
        {
            var userId = User.GetUserId();

            var result = await _companyRequestService.Create(userId!, createCompanyRequestDto);

            if(!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status409Conflict => Conflict(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Value?.ToCompanyRequestDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateCompanyRequest)]
        public async Task<ActionResult<ApiDataResponse<CompanyRequestDto>>> UpdateStatus([FromRoute] int id, [FromBody] UpdateCompanyRequestDto updateCompanyRequestDto)
        {
            var result = await _companyRequestService.UpdateStatus(id, updateCompanyRequestDto);

            if(!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToCompanyRequestDto());
        }
    }
}
