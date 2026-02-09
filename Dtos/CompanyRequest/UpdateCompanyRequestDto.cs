using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public record UpdateCompanyRequestDto
    {
        [Required]
        [ValidEnum(typeof(CompanyRequestStatus), ErrorMessage = ValidationMessages.CompanyRequestStatus)]
        public CompanyRequestStatus Status { get; init; }

        public string Reason { get; init; } = "";
    }
}
