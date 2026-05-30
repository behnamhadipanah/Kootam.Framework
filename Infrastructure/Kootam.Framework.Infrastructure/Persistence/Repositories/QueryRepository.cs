using Kootam.Framework.Abstractions;
using Kootam.Framework.Domain.Contracts;
using Kootam.Framework.Infrastructure.Extensions;
using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kootam.Framework.Infrastructure.Persistence.Repositories;

public class QueryRepository<TEntity, TKey, TDbContext>
    
    : RepositoryBase<TEntity, TKey, TDbContext>
    , IQueryRepository<TEntity, TKey>
       where TEntity : class,IHasId<TKey>
    where TDbContext:DbContext
    where TKey : struct, IComparable,
                        IComparable<TKey>,
                        IConvertible,
                        IEquatable<TKey>,
                        IFormattable
{
    private readonly BaseDbContext<TDbContext> _context;


    public QueryRepository(BaseDbContext<TDbContext> context):base(context)
    {
        _context = context;
    }

    protected IQueryable<TEntity> Query=> _context.Set<TEntity>().AsNoTracking();


    #region GetAll
    public IEnumerable<TEntity> GetAll()
            => Query.ToList();


    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    => await Query.ToListAsync(cancellationToken);

    public IEnumerable<TEntity> GetAllGraph()
    {
        IQueryable<TEntity> query = GetGraphPaths().AsNoTracking();

        return query.ToList();


    }

    public async Task<IEnumerable<TEntity>> GetAllGraphAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = GetGraphPaths().AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }
    #endregion

    #region Get
    #region Async
    public async Task<TEntity> GetByAsync(TKey id, CancellationToken cancellationToken = default)
        =>await Query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public async Task<TEntity> GetGraphByAsync(TKey id, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = GetGraphPaths().AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }


    #endregion

    #region Sync

    public TEntity GetBy(TKey id) => Query.FirstOrDefault(x => x.Id.Equals(id));

    public TEntity GetGraphBy(TKey id)
    {
        IQueryable<TEntity> query = GetGraphPaths().AsNoTracking();

        return query.FirstOrDefault(x => x.Id.Equals(id));
    }

    #endregion


    #endregion

    #region Expression
    public IEnumerable<TEntity> GetByExpression(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] joins)
        => Query.IncludeMany(joins).Where(predicate).ToList();


    public async Task<IList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] joins)
    => await Query.IncludeMany(joins).Where(predicate).ToListAsync(cancellationToken);


    #endregion



}
