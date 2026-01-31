using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public class CompanyRequestQueryDto : BaseQueryDto
    {
        [MaxLength(50, ErrorMessage = "Can not exceed 50 characters")]
        public string? CompanyName { get; set; }

        [MaxLength(50, ErrorMessage = "Can not exceed 50 characters")]
        public string? TIN { get; set; }

        [ValidEnum(typeof(CompanyRequestStatus), ErrorMessage = "Invalid status value.")]
        public CompanyRequestStatus? Status { get; set; }

        public string? UserId { get; set; }
    }
}
