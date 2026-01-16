namespace JobNexus.Interfaces
{
    public interface IEntityTimestamps
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
