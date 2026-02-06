namespace JobNexus.Dtos.Company
{
    public class CompanyDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string Description { get; set; } = "";

        public string TIN { get; set; } = "";

        public string BusinessLicenseUrl { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
