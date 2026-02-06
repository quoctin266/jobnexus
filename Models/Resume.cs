using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class Resume : IEntityTimestamps, ISoftDelete
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; } = "";

        public bool IsDefault { get; set; }

        public string AppUserId { get; set; } = "";

        public AppUser? AppUser { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
    }
}
