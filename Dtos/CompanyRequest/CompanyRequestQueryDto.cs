using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public class CompanyRequestQueryDto : BaseQueryDto
    {
        [MaxLength(50, ErrorMessage = ValidationMessages.CompanyNameMaxLength)]
        public string? CompanyName { get; set; }

        [MaxLength(50, ErrorMessage = ValidationMessages.TINMaxLength)]
        public string? TIN { get; set; }

        [ValidEnum(typeof(CompanyRequestStatus), ErrorMessage = ValidationMessages.CompanyRequestStatus)]
        public CompanyRequestStatus? Status { get; set; }

        public string? UserId { get; set; }
    }
}
