using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class Skill : IEntityTimestamps, ISoftDelete
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Name { get; set; } = "";

        public bool IsActive { get; set; } = true;

        public List<Job> Jobs { get; set; } = [];

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
    }
}
