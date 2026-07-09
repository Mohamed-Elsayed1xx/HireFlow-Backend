namespace HireFlow.Application.Common.Models;

public class Result<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public string? ErrorCode { get; private set; }

    private Result() { }

    public static Result<T> Ok(T data, string message = "Operation completed successfully")
        => new() { Success = true, Data = data, Message = message };

    public static Result<T> Fail(string errorCode, string message)
        => new() { Success = false, ErrorCode = errorCode, Message = message };
}

public class Result
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public string? ErrorCode { get; private set; }

    private Result() { }

    public static Result Ok(string message = "Operation completed successfully")
        => new() { Success = true, Message = message };

    public static Result Fail(string errorCode, string message)
        => new() { Success = false, ErrorCode = errorCode, Message = message };
}