using JobNexus.Common.Enum;
using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class CompanyRequest : IEntityTimestamps
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string Description { get; set; } = "";

        [Column(TypeName = "varchar(50)")]
        public string TIN { get; set; } = "";

        public string BusinessLicenseUrl { get; set; } = "";

        public string EmploymentContractUrl { get; set; } = "";

        public CompanyRequestStatus Status { get; set; }

        public string Reason { get; set; } = "";

        public string AppUserId { get; set; } = "";

        public AppUser? AppUser { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
