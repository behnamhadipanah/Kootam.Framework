using Kootam.Extensions.Cqrs.Abstractions.Enums;

namespace Kootam.Extensions.Cqrs.Abstractions.Models;

public class Result
{
    public bool IsSuccess { get; }
    public ResultStatus Status { get; }
    public IReadOnlyList<string> Messages { get; }

    protected Result(bool isSuccess, ResultStatus status, IEnumerable<string>? messages = null)
    {
        IsSuccess = isSuccess;
        Status = status;
        Messages = messages?.ToList() ?? new List<string>();
    }

    public static Result Success(string? message = null)
        => new(true, ResultStatus.Success,
            message is null ? null : new[] { message });

    public static Result Failure(ResultStatus status, params string[] messages)
        => new(false, status, messages);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T data, IEnumerable<string>? messages = null)
        : base(true, ResultStatus.Success, messages)
    {
        Data = data;
    }

    private Result(ResultStatus status, IEnumerable<string>? messages)
        : base(false, status, messages)
    {
    }

    public static Result<T> Success(T data, string? message = null)
        => new(data, message is null ? null : new[] { message });

    public static new Result<T> Failure(ResultStatus status, params string[] messages)
        => new(status, messages);
}
