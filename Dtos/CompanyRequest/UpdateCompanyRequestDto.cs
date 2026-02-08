using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public class UpdateCompanyRequestDto
    {
        [Required]
        [ValidEnum(typeof(CompanyRequestStatus), ErrorMessage = ValidationMessages.CompanyRequestStatus)]
        public CompanyRequestStatus Status { get; set; }

        public string Reason { get; set; } = "";
    }
}
