using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Resume
{
    public record ResumeQueryDto : BaseQueryDto
    {
        [MaxLength(50, ErrorMessage = ValidationMessages.ResumeTitleMaxLength)]
        public string? Title { get; init; }

        public string? UserId { get; init; }
    }
}
