using JobNexus.Common.Enum;
using JobNexus.Interfaces;
using System.ComponentModel;

namespace JobNexus.Models
{
    public class CompanyEmployee : IEntityTimestamps
    {
        public int Id { get; set; }

        public CompanyRole CompanyRole { get; set; }

        public string EmploymentContractUrl { get; set; } = "";

        public bool IsActive { get; set; }

        public int CompanyId { get; set; }

        public Company? Company { get; set; }

        public string AppUserId { get; set; } = "";

        public AppUser? AppUser { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
