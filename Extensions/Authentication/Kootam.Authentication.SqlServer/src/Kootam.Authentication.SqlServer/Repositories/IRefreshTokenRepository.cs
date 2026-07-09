using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.SqlServer.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token,CancellationToken cancellationToken);

    Task<RefreshToken?> FindAsync(string token,CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}