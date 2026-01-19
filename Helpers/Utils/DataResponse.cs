namespace JobNexus.Helpers.Utils
{
    public class DataResponse<T>
    {
        public int statusCode { get; set; }

        public string message { get; set; } = "";

        public T? data { get; set; }
    }

    public sealed class VoidType { }
}

