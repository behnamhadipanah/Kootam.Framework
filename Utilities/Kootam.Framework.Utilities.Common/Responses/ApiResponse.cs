using Microsoft.AspNetCore.Http;

namespace Kootam.Framework.Utilities.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T? data = default, string? message = null)
        => new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Data = data,
            Message = message
        };

    public static ApiResponse<T> Fail(
        int statusCode,
        string message)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };
}
