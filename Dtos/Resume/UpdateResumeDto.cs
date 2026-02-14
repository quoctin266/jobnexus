using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Resume
{
    public record UpdateResumeDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.ResumeTitleMaxLength)]
        public string Title { get; init; } = "";

        [Required]
        public bool IsDefault { get; init; }

        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public IFormFile? ResumeFile { get; init; }
    }
}
