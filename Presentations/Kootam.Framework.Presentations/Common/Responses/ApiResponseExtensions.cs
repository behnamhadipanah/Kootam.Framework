using Kootam.Framework.Utilities.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Framework.Presentations.Common.Responses;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
        => new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
}