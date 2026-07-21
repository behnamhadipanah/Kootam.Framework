using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface IAccessTokenReader
{
    ValueTask<string?> ReadAsync(HttpContext context);

}
