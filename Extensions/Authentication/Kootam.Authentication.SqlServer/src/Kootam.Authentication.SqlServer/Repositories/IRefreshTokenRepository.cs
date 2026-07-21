using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.SqlServer.Repositories;

public interface IRefreshTokenRepository<TUserKey>
{
    Task AddAsync(RefreshToken<TUserKey> token,CancellationToken cancellationToken);

    Task<RefreshToken<TUserKey>?> FindAsync(string token,CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}