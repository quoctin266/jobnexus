using JobNexus.Interfaces;

namespace JobNexus.Helpers.Utils
{
    public record ServiceResult<T> : IErrorResponse
    {
        public bool IsSuccess { get; init; }

        public T? Value { get; init; }

        public int StatusCode { get; init; }

        public string Error { get; init; } = "";

        public List<string> Messages { get; init; } = [];

        public static ServiceResult<T> Success(T value) => new()
        {
            IsSuccess = true,
            Value = value
        };

        public static ServiceResult<T> Failure(int code, string error, List<string> messages) => new()
        {
            IsSuccess = false,
            StatusCode = code,
            Error = error,
            Messages = messages
        };
    }
}
