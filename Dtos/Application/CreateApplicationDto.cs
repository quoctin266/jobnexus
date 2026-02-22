using JobNexus.Common.Constant.Messages;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Application
{
    public record CreateApplicationDto
    {
        [Required]
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = ValidationMessages.PhoneNumberFormat)]
        public string PhoneNumber { get; set; } = "";

        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.ApplicationFullNameMaxLength)]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        [MaxLength(20, ErrorMessage = ValidationMessages.EmailMaxLength)]
        public string Email { get; set; } = "";

        public string? Intro { get; set; }

        [Required]
        public int JobId { get; init; }

        [Required]
        public int ResumeVersionId { get; set; }
    }
}
