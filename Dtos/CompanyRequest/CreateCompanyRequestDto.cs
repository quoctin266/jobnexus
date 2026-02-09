using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public record CreateCompanyRequestDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.CompanyNameMaxLength)]
        public string Name { get; init; } = "";

        [Required]
        public string Address { get; init; } = "";

        [Required]
        public string Description { get; init; } = "";

        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.TINMaxLength)]
        public string TIN { get; init; } = "";

        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile BusinessLicense { get; init; }

        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile EmploymentContract { get; init; }
    }
}
