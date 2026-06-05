using Microsoft.AspNetCore.Mvc;
using System.Net;
using Kootam.Cqrs.Abstractions.Enums;
using Kootam.Cqrs.Abstractions.Models;

using Kootam.Framework.Utilities.Responses;


namespace Kootam.Framework.Presentations.Responses;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
        => new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };



    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        var response = new ApiResponse<T>
        {
            Success = result.IsSuccess,
            Data = result.Data,
            Message = result.Messages.FirstOrDefault()
        };
        return result.Status.GetObjectResult(response);


    }

    public static IActionResult ToActionResult(this Result result)
    {
        var response = new ApiResponse
        {
            Success = result.IsSuccess,
            Message = result.Messages.FirstOrDefault()
        };
        return result.Status.GetObjectResult(response);
        

    }

    private static ObjectResult GetObjectResult(this ResultStatus result,object response)
    {
        return result switch
        {
            ResultStatus.Success => new OkObjectResult(response),
            ResultStatus.NotFound => new NotFoundObjectResult(response),
            ResultStatus.ValidationError => new BadRequestObjectResult(response),
            ResultStatus.Forbidden => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Forbidden },
            ResultStatus.Conflict => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Conflict },
            ResultStatus.Unauthorized => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Unauthorized },
            _ => new ObjectResult(response) { StatusCode = 500 }
        };
    }
}