namespace JobNexus.Dtos.Skill
{
    public record SkillDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = "";

        public bool IsActive { get; init; } = true;

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset UpdatedAt { get; init; }
    }
}
