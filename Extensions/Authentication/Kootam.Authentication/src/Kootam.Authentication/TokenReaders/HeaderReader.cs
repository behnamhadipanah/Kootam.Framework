using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.TokenReaders;

public sealed class HeaderReader : ITokenReader
{
    public ValueTask<string?> ReadAsync(HttpContext context)
    {
        var header =
            context.Request.Headers["Authorization"]
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(header))
            return ValueTask.FromResult<string?>(null);

        if (!header.StartsWith("Bearer "))
            return ValueTask.FromResult<string?>(null);

        return ValueTask.FromResult<string?>(
            header["Bearer ".Length..]);
    }
}