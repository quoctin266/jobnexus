using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.Company;
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
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ResponseMessage(message: SuccessMessages.FetchListCompany)]
        public async Task<ActionResult<ApiDataResponse<QueryResponse<CompanyDto>>>> GetList([FromQuery] CompanyQueryDto companyQueryDto)
        {
            var result = await _companyService.GetAll(companyQueryDto);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseMessage(message: SuccessMessages.FetchOneCompany)]
        public async Task<ActionResult<ApiDataResponse<CompanyDto>>> GetById([FromRoute] int id)
        {
            var result = await _companyService.FindById(id);

            if (!result.IsSuccess)
            {
                var response = new ErrorResponse(result.Error, result.Messages);

                return result.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response)
                };
            }

            return Ok(result.Value?.ToCompanyDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPut("{id}")]
        [ResponseMessage(message: SuccessMessages.UpdateCompany)]
        public async Task<ActionResult<ApiDataResponse<CompanyDto>>> Update([FromRoute] int id, [FromBody] UpdateCompanyDto updateCompanyDto)
        {
            var result = await _companyService.Update(id, updateCompanyDto, User);

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

            return Ok(result.Value?.ToCompanyDto());
        }

        [Authorize(Roles = "Employer")]
        [HttpPatch("close/{id}")]
        [ResponseMessage(message: SuccessMessages.CloseCompany)]
        public async Task<ActionResult<ApiDataResponse<CompanyDto>>> UpdateToInactive([FromRoute] int id)
        {
            var result = await _companyService.UpdateToInactive(id, User);

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

            return Ok(result.Value?.ToCompanyDto());
        }
    }
}
