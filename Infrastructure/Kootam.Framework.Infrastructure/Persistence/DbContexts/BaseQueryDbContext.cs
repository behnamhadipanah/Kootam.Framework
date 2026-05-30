using Microsoft.EntityFrameworkCore;

namespace Kootam.Framework.Infrastructure.Persistence.DbContexts;

public class BaseQueryDbContext<TDbContext> : BaseDbContext<TDbContext>
    where TDbContext:DbContext
{
    public BaseQueryDbContext(DbContextOptions<TDbContext> options):base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

    }
    public override int SaveChanges()
    {
        throw new NotImplementedException();
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotImplementedException();
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
