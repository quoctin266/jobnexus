using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.User
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = ValidationMessages.UsernameMaxLength)]
        public string Username { get; set; } = "";

        [Required]
        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public string DateOfBirth { get; set; } = "";

        [Required]
        [ValidEnum(typeof(Gender), ErrorMessage = ValidationMessages.GenderValue)]
        public string Gender { get; set; } = "";

        [Required]
        [MaxLength(250, ErrorMessage = ValidationMessages.AddressMaxLength)]
        public string Address { get; set; } = "";

        [Required]
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = ValidationMessages.PhoneNumberFormat)]
        public string PhoneNumber { get; set; } = "";
    }
}
