namespace HireFlow.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

public record ValidationError(string Field, string Message);