using JobNexus.Interfaces;

namespace JobNexus.Helpers.Utils
{
    public record ErrorResponse (string Error, List<string> Messages) : IErrorResponse;
}
