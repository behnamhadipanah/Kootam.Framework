using Kootam.Authentication.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace Kootam.Authentication.SqlServer.Context;

public interface IRefreshTokenDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}