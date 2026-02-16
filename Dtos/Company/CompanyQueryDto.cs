using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Company
{
    public record CompanyQueryDto : BaseQueryDto
    {
        [MaxLength(50, ErrorMessage = ValidationMessages.CompanyNameMaxLength)]
        public string? Name { get; init; }

        [MaxLength(50, ErrorMessage = ValidationMessages.TINMaxLength)]
        public string? TIN { get; init; }

        public bool? IsActive { get; init; }
    }
}
