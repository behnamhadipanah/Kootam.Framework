using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Services;

namespace Kootam.Authentication.Redis.Services;

public sealed class RedisRefreshTokenService:IRefreshTokenService
{
    public Task StoreAsync(RefreshToken refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> FindAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}