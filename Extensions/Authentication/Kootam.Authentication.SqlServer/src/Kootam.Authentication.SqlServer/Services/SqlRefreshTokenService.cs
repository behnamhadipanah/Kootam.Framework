using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.SqlServer.Repositories;

namespace Kootam.Authentication.SqlServer.Services;

public class SqlRefreshTokenService<TUserKey>(IRefreshTokenRepository<TUserKey> refreshTokenRepository) : IRefreshTokenService<TUserKey>
{
    public async Task StoreAsync(RefreshToken<TUserKey> refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        await refreshTokenRepository.AddAsync(
            new RefreshToken<TUserKey>
            {
                Id = Guid.NewGuid(),
                UserId=refreshToken.UserId,
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

        if (entity is null)
            return;

        entity.Revoked = DateTime.UtcNow;

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken<TUserKey>?> FindAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
    {
        var entity =
            await refreshTokenRepository.FindAsync(
                refreshToken,
                cancellationToken);

        if (entity is null)
            return null;

        return new RefreshToken<TUserKey>
        {
            Token = entity.Token,
            UserId=entity.UserId,
            Created = entity.Created,
            CreatedByIp = entity.CreatedByIp,
            Expires = entity.Expires,
            Revoked = entity.Revoked,
            RevokedByIp = entity.RevokedByIp,
            ReplacedByToken = entity.ReplacedByToken
        };
    }

    public async Task RotateAsync(RefreshToken<TUserKey> oldToken, RefreshToken<TUserKey> newToken,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var entity = await refreshTokenRepository.FindAsync(oldToken.Token, cancellationToken);

        if (entity is null)
            throw new InvalidOperationException("Refresh token not found.");

        entity.Revoked = DateTime.UtcNow;
        entity.RevokedByIp = oldToken.RevokedByIp;
        entity.ReplacedByToken = newToken.Token;

        await refreshTokenRepository.AddAsync(new RefreshToken<TUserKey>
        {
            Id = Guid.NewGuid(),
            UserId = newToken.UserId,
            Token = newToken.Token,
            Created = newToken.Created,
            CreatedByIp = newToken.CreatedByIp,
            Expires = newToken.Expires
        }, cancellationToken);

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}