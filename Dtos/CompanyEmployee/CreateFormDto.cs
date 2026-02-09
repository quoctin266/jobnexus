using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyEmployee
{
    public record CreateFormDto
    {
        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile EmploymentContract { get; init; }

        [Required]
        public int CompanyId { get; init; }

        [Required]
        public required string AppUserId { get; init; }

        [Required]
        [ValidEnum(typeof(CompanyRole), ErrorMessage = ValidationMessages.CompanyRoleValue)]
        public CompanyRole CompanyRole { get; init; }
    }
}
