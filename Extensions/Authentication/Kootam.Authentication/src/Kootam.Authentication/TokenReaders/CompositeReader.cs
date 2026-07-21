using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.TokenReaders;

public sealed class CompositeReader:IAccessTokenReader
{
    private readonly IEnumerable<IAccessTokenReader> _tokenReads;

    public CompositeReader(IEnumerable<IAccessTokenReader> tokenReads)
    {
        _tokenReads = tokenReads;
    }
    
    public async ValueTask<string?> ReadAsync(HttpContext context)
    {
        foreach (var tokenRead in _tokenReads)
        {
            var token=await tokenRead.ReadAsync(context);
            if (!string.IsNullOrWhiteSpace(token))
                return token;
        }
        return null;
    }
}