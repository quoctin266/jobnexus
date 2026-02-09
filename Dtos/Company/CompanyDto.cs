namespace JobNexus.Dtos.Company
{
    public record CompanyDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = "";

        public string Address { get; init; } = "";

        public string Description { get; init; } = "";

        public string TIN { get; init; } = "";

        public string BusinessLicenseUrl { get; init; } = "";

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset UpdatedAt { get; init; }
    }
}
