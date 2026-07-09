using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.SqlServer.Repositories;

namespace Kootam.Authentication.SqlServer.Services;

public class SqlRefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public async Task StoreAsync(RefreshToken refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        await refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken.Token,
                Created = refreshToken.Created,
                CreatedByIp = refreshToken.CreatedByIp,
                Expires = refreshToken.Expires,
                Revoked = refreshToken.Revoked,
                RevokedByIp = refreshToken.RevokedByIp,
                ReplacedByToken = refreshToken.ReplacedByToken
            },
            cancellationToken);
        try
        {
            await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        var entity =
            await refreshTokenRepository.FindAsync(
                refreshToken,
                cancellationToken);

        if (entity == null)
            return;

        entity.Revoked = DateTime.UtcNow;

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> FindAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        var entity =
            await refreshTokenRepository.FindAsync(
                refreshToken,
                cancellationToken);

        if (entity == null)
            return null;

        return new RefreshToken
        {
            Token = entity.Token,
            Created = entity.Created,
            CreatedByIp = entity.CreatedByIp,
            Expires = entity.Expires,
            Revoked = entity.Revoked,
            RevokedByIp = entity.RevokedByIp,
            ReplacedByToken = entity.ReplacedByToken
        };
    }

    public Task RotateAsync(RefreshToken oldToken, RefreshToken newToken,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}