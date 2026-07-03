namespace GymBookingSystem.API.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; set; } = 500;

    public AppException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, 401) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, 403) { }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, 400) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; set; }

    public ValidationException(IDictionary<string, string[]> errors) : base("Validation failed", 422)
    {
        Errors = errors;
    }

    public ValidationException(string field, string message) : base("Validation failed", 422)
    {
        Errors = new Dictionary<string, string[]> { { field, new[] { message } } };
    }
}
