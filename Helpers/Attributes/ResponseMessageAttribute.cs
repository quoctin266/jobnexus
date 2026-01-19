namespace JobNexus.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ResponseMessageAttribute : Attribute
    {
        public string Message { get; }

        public ResponseMessageAttribute(string message)
        {
            Message = message;
        }
    }
}

