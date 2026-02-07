namespace JobNexus.Helpers.Utils
{
    public class ApiDataResponse<T>
    {
        public int statusCode { get; init; }

        public string message { get; init; } = "";

        public T? data { get; init; }
    }

    public sealed class VoidType { }
}

