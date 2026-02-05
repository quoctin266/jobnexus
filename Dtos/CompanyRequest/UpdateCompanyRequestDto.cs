using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public class UpdateCompanyRequestDto
    {
        [Required]
        [ValidEnum(typeof(CompanyRequestStatus), ErrorMessage = "Invalid status value.")]
        public CompanyRequestStatus Status { get; set; }

        public string Reason { get; set; } = "";
    }
}
