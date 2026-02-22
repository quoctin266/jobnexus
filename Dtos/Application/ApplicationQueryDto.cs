using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Application
{
    public record ApplicationQueryDto : BaseQueryDto
    {
        public string? PhoneNumber { get; set; }

        [MaxLength(50, ErrorMessage = ValidationMessages.ApplicationFullNameMaxLength)]
        public string? FullName { get; set; }

        [MaxLength(20, ErrorMessage = ValidationMessages.EmailMaxLength)]
        public string? Email { get; set; }

        public int? JobId { get; init; }

        public string? UserId { get; init; }

        public ApplicationStatus? Status { get; init; }
    }
}
