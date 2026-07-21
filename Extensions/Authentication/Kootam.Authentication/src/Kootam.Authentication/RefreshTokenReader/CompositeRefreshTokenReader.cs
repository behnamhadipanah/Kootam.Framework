using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.RefreshTokenReader;

public sealed class CompositeRefreshTokenReader : IRefreshTokenReader
{
    private readonly IEnumerable<IRefreshTokenReader> _readers;

    public CompositeRefreshTokenReader(
        IEnumerable<IRefreshTokenReader> readers)
    {
        _readers = readers;
    }

    public async ValueTask<string?> ReadAsync(HttpContext context)
    {
        foreach (var reader in _readers)
        {
            var token = await reader.ReadAsync(context);

            if (!string.IsNullOrWhiteSpace(token))
                return token;
        }

        return null;
    }
}
