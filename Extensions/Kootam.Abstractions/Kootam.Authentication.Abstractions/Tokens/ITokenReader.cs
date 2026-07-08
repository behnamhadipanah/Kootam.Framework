using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface ITokenReader
{
    ValueTask<string?> ReadAsync(HttpContext context);

}