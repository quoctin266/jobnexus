namespace JobNexus.Helpers.Utils
{
    public record BaseQueryDto
    {
        public string? SortBy { get; init; }

        public bool IsDescending { get; init; } = false;

        public int PageNumber { get; init; } = 1;

        public int PageSize { get; init; } = 10;
    }
}
