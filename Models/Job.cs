using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class Job : IEntityTimestamps, ISoftDelete
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = "";

        [Column(TypeName = "varchar(20)")]
        public string Location { get; set; } = "";

        public decimal SalaryMin { get; set; }

        public decimal SalaryMax { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Level { get; set; } = "";

        public string Description { get; set; } = "";

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int CompanyId { get; set; }

        public Company? Company { get; set; }

        public int CompanyEmployeeId { get; set; } 

        public CompanyEmployee? CompanyEmployee { get; set; }

        public List<Skill> Skills { get; set; } = [];

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
    }
}
