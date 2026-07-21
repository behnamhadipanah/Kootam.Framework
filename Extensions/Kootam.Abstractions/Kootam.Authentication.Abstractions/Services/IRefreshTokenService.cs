using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Services;

public interface IRefreshTokenService<TUserKey>
{
    Task StoreAsync(RefreshToken<TUserKey> refreshToken,CancellationToken cancellationToken = default);

    Task RevokeAsync(string refreshToken,CancellationToken cancellationToken = default);

    Task<RefreshToken<TUserKey>?> FindAsync(string refreshToken,CancellationToken cancellationToken = default);
    Task RotateAsync(RefreshToken<TUserKey> oldToken,RefreshToken<TUserKey> newToken, CancellationToken cancellationToken = default);
}