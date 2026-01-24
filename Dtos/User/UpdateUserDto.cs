using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.User
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string Username { get; set; } = "";

        [Required]
        [Iso8601Date(ErrorMessage = "DateOfBirth must be a valid ISO 8601 string.")]
        public string DateOfBirth { get; set; } = "";

        [Required]
        [ValidEnum(typeof(Gender), ErrorMessage = "Invalid gender value.")]
        public string Gender { get; set; } = "";

        [Required]
        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = "";

        [Required]
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; } = "";
    }
}
