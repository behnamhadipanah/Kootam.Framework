using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Services;

public interface IRefreshTokenService
{
    Task StoreAsync(RefreshToken refreshToken,CancellationToken cancellationToken = default);

    Task RevokeAsync(string refreshToken,CancellationToken cancellationToken = default);

    Task<RefreshToken?> FindAsync(string refreshToken,CancellationToken cancellationToken = default);
}