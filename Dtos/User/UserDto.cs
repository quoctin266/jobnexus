namespace JobNexus.Dtos.User
{
    public record UserDto
    {
        public string Id { get; init; } = "";

        public string Username { get; init; } = "";

        public string Email { get; init; } = "";

        public DateTime DateOfBirth { get; init; }

        public string Gender { get; init; } = "";

        public string Address { get; init; } = "";

        public string PhoneNumber { get; init; } = "";
    }
}
