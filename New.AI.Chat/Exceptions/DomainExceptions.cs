namespace New.AI.Chat.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class ValidationException : DomainException
{
    public IEnumerable<string>? Errors { get; }
    public ValidationException(string message) : base(message) { }
    public ValidationException(IEnumerable<string> errors) : base("Validation errors") { Errors = errors; }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message) { }
}
