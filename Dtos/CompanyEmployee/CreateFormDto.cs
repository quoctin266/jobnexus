using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyEmployee
{
    public class CreateFormDto
    {
        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile EmploymentContract { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public required string AppUserId { get; set; }

        [Required]
        [ValidEnum(typeof(CompanyRole), ErrorMessage = ValidationMessages.CompanyRoleValue)]
        public CompanyRole CompanyRole { get; set; }
    }
}
