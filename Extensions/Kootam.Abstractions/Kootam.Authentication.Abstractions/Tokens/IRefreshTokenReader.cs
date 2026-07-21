using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface IRefreshTokenReader
{
    ValueTask<string?> ReadAsync(HttpContext context);
}