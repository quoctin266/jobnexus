using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class Company : IEntityTimestamps
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string Description { get; set; } = "";

        [Column(TypeName = "varchar(50)")]
        public string TIN { get; set; } = "";

        public string BusinessLicenseUrl { get; set; } = "";

        public bool IsActive { get; set; } = true;

        public List<CompanyEmployee> CompanyEmployees { get; set; } = [];

        public List<Job> Jobs { get; set; } = [];

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
