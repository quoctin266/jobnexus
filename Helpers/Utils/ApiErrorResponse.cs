namespace JobNexus.Helpers.Utils
{
    public class ApiErrorResponse
    {
        public int statusCode { get; set; }

        public List<string> message { get; set; } = [];

        public string error { get; set; } = "";
    }
}
