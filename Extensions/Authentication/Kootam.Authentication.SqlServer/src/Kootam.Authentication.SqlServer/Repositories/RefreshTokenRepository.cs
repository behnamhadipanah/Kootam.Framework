using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Kootam.Authentication.SqlServer.Repositories;

public class RefreshTokenRepository(IRefreshTokenDbContext context)
    : IRefreshTokenRepository
{

    public async Task AddAsync(
        RefreshToken token,
        CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(
            token,
            cancellationToken);
    }

    public Task<RefreshToken?> FindAsync(
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