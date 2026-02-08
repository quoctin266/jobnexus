using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.CompanyRequest
{
    public class CreateCompanyRequestDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.CompanyNameMaxLength)]
        public string Name { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.TINMaxLength)]
        public string TIN { get; set; } = "";

        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile BusinessLicense { get; set; }

        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile EmploymentContract { get; set; }
    }
}
