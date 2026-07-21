using Kootam.Authentication.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace Kootam.Authentication.SqlServer.Context;

public interface IRefreshTokenDbContext<TUserKey>
{
    DbSet<RefreshToken<TUserKey>> RefreshTokens { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}