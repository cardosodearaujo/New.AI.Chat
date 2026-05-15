namespace New.AI.Chat.Shared
{
    public record Result<T>
    {
        public bool IsSuccess { get; init; }
        public bool IsNotFound { get; init; }
        public IEnumerable<string>? Errors { get; init; }
        public T? Data { get; init; }

        public static Result<T> Success(T data) => new Result<T> { IsSuccess = true, Data = data };

        public static Result<T> Failure(IEnumerable<string> errors) => new Result<T> { IsSuccess = false, Errors = errors ?? Enumerable.Empty<string>() };

        public static Result<T> NotFound(string? message = null) => new Result<T> { IsSuccess = false, IsNotFound = true, Errors = message is null ? Enumerable.Empty<string>() : new[] { message } };
    }
}
