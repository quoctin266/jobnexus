using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Resume
{
    public record CreateResumeDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.ResumeTitleMaxLength)]
        public string Title { get; init; } = "";

        [Required]
        public bool IsDefault { get; init; }

        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile ResumeFile { get; init; }
    }
}
