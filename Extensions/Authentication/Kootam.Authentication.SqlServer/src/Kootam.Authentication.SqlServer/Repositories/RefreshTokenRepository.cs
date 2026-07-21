using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Kootam.Authentication.SqlServer.Repositories;

public class RefreshTokenRepository<TUserKey>(IRefreshTokenDbContext<TUserKey> context)
    : IRefreshTokenRepository<TUserKey>
{

    public async Task AddAsync(
        RefreshToken<TUserKey> token,
        CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(
            token,
            cancellationToken);
    }

    public Task<RefreshToken<TUserKey>?> FindAsync(
        string token,
        CancellationToken cancellationToken)
    {
        return context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.Token == token,
                cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(
            cancellationToken);
    }
}