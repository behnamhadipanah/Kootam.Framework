using Kootam.Framework.Authentication.Jwt.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Kootam.Framework.Authentication.Jwt.Middlewares;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITokenValidator tokenValidator)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(' ').Last();

        if (!string.IsNullOrEmpty(token))
        {
            //var principal = await tokenValidator.ValidateTokenAsync(token, context);

            //if (principal != null)
            //{
            //    context.User = principal;
            //}
        }

        await _next(context);
    }
}
