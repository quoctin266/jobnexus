using JobNexus.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public record UpdateCompanyRequestDto
    {
        [Required]
        public CompanyRequestStatus Status { get; init; }

        public string Reason { get; init; } = "";
    }
}

