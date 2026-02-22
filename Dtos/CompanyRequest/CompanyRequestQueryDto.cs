using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public record CompanyRequestQueryDto : BaseQueryDto
    {
        [MaxLength(50, ErrorMessage = ValidationMessages.CompanyNameMaxLength)]
        public string? CompanyName { get; init; }

        [MaxLength(50, ErrorMessage = ValidationMessages.TINMaxLength)]
        public string? TIN { get; init; }

        public CompanyRequestStatus? Status { get; init; }

        public string? UserId { get; init; }
    }
}
