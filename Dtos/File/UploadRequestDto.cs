using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.File
{
    public record UploadRequestDto
    {
        [Required]
        [ValidExtensionsAttribute([".pdf", ".docx", ".jpg", ".png", ".jpeg"])]
        public required IFormFile File { get; init; }
    }
}
