using JobNexus.Common.Enum;
using JobNexus.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobNexus.Models
{
    public class Application : IEntityTimestamps
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string PhoneNumber { get; set; } = "";

        [Column(TypeName = "varchar(50)")]
        public string FullName { get; set; } = "";

        [Column(TypeName = "varchar(20)")]
        public string Email { get; set; } = "";

        public string Intro { get; set; } = "";

        public int JobId { get; set; }

        public Job? Job { get; set; }

        public int ResumeVersionId { get; set; }

        public ResumeVersion? ResumeVersion { get; set; }

        public string AppUserId { get; set; } = "";

        public AppUser? AppUser { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
