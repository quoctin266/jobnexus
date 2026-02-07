namespace JobNexus.Helpers.Utils
{
    public record ApiErrorResponse(int StatusCode, List<string> Message, string Error);
}
