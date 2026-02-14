using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class ResumeVersion : IEntityTimestamps
    {
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VersionNo { get; set; }

        public string FileUrl { get; set; } = "";

        public int ResumeId { get; set; }

        public Resume? Resume { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
