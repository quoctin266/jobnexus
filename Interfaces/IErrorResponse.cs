namespace JobNexus.Interfaces
{
    public interface IErrorResponse
    {
        public string Error { get; init; }

        public List<string> Messages { get; init; }
    }
}
