namespace JobNexus.Interfaces
{
    public interface IEntityTimestamps
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
