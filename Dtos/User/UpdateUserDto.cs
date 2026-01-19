using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.User
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string Username { get; set; } = "";

        [Required]
        [Range(16, 100)]
        public int Age { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = "";

        [Required]
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; } = "";
    }
}
