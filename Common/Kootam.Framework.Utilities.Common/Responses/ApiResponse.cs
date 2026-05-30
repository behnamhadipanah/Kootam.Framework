using Microsoft.AspNetCore.Http;

namespace Kootam.Framework.Utilities.Responses;

public class ApiResponse
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string? Message { get; init; }

    public static ApiResponse Ok(string? message = null)
        => new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message
        };

    public static ApiResponse Fail(int statusCode, string message)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };

    public static ApiResponse Fail(string message)
        => new()
        {
            Success = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Message = message
        };
}

public class ApiResponse<T>:ApiResponse
{
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Data = data,
            Message = message
        };

}



