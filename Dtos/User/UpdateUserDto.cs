using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.User
{
    public record UpdateUserDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = ValidationMessages.UsernameMaxLength)]
        public string Username { get; init; } = "";

        [Required]
        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public string DateOfBirth { get; init; } = "";

        [Required]
        public string Gender { get; init; } = "";

        [Required]
        [MaxLength(250, ErrorMessage = ValidationMessages.AddressMaxLength)]
        public string Address { get; init; } = "";

        [Required]
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = ValidationMessages.PhoneNumberFormat)]
        public string PhoneNumber { get; init; } = "";
    }
}
