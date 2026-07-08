using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenStore _tokenStore;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor, ITokenStore tokenStore, IRefreshTokenService refreshTokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenStore = tokenStore;
        _refreshTokenService = refreshTokenService;
    }

    public async Task SignInAsync(IssuedToken token, CancellationToken cancellationToken = new CancellationToken())
    {
        var context = _httpContextAccessor.HttpContext
                      ?? throw new InvalidOperationException("HttpContext not found.");

        await _tokenStore.SignInAsync(context, new TokenStoreOption()
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            AccessTokenExpires = token.AccessTokenExpires,
            RefreshTokenExpires = token.RefreshTokenExpires,
        });

        if (token.RefreshToken is not null)
        {
            await _refreshTokenService.StoreAsync(
                token.RefreshToken,
                cancellationToken);
        }
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var context =
            _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();

        await _tokenStore.SignOutAsync(context);
    }
}
