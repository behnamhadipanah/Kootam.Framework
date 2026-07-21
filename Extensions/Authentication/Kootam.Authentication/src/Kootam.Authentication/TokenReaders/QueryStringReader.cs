using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.TokenReaders;

public sealed class QueryStringReader : IAccessTokenReader
{
    public ValueTask<string?> ReadAsync(HttpContext context)
    {
        var token = context.Request.Query["access_token"].FirstOrDefault();

        return ValueTask.FromResult(token);
    }
}