using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Services;

namespace Kootam.Authentication.Redis.Services;

public sealed class RedisRefreshTokenService<TUserKey>:IRefreshTokenService<TUserKey>
{
    public Task StoreAsync(RefreshToken<TUserKey> refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken<TUserKey>?> FindAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task RotateAsync(RefreshToken<TUserKey> oldToken, RefreshToken<TUserKey> newToken,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}