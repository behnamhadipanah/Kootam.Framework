using Kootam.Framework.Domain.Entities;
using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kootam.Framework.Infrastructure.Persistence.Repositories;

public class RepositoryBase<TEntity, TKey, TDbContext>
    where TEntity : class
    where TDbContext : DbContext
    where TKey : struct
    , IComparable,
IComparable<TKey>,
IConvertible,
IEquatable<TKey>,
IFormattable
{
    protected readonly BaseDbContext<TDbContext> Context;
    protected readonly DbSet<TEntity> Set;
    protected RepositoryBase(BaseDbContext<TDbContext> context)
    {
        Context = context;
        Set = Context.Set<TEntity>();
    }

    protected IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] includes)
    {
        if (includes == null || includes.Length == 0)
            return query;

        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }

    protected IQueryable<TEntity> GetGraphPaths()
    {
        var graphPaths = Context.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = Context.Set<TEntity>().AsQueryable();
        foreach (var graphPath in graphPaths)
        {
            query = query.Include(graphPath);
        }

        return query;
    }
}
