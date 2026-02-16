using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Company
{
    public record UpdateCompanyDto
    {
        [Required]
        public string Address { get; init; } = "";

        [Required]
        public string Description { get; init; } = "";
    }
}
