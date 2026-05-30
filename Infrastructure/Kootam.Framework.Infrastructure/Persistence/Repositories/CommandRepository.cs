using Kootam.Framework.Domain.Contracts;
using Kootam.Framework.Domain.Entities;
using Kootam.Framework.Domain.ValueObjects;
using Kootam.Framework.Infrastructure.Extensions;
using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Kootam.Framework.Infrastructure.Persistence.Repositories;


public class CommandRepository<TAggregate, TKey,TDbContext>
     : RepositoryBase<TAggregate, TKey, TDbContext>, ICommandRepository<TAggregate, TKey>
    where TAggregate : AggregateRoot<TKey>
    where TDbContext : DbContext
     where TKey : struct,
          IComparable,
          IComparable<TKey>,
          IConvertible,
          IEquatable<TKey>,
          IFormattable
{
    private readonly BaseDbContext<TDbContext> _context;

    public CommandRepository(BaseDbContext<TDbContext> context):base(context)
    {
        _context = context;
    }

    #region Add
    public void Add(TAggregate entity) => _context.Add<TAggregate>(entity);
    public async Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default) => await _context.AddAsync<TAggregate>(entity, cancellationToken);

    #endregion

    #region Exists
    public bool Exists(Expression<Func<TAggregate, bool>> expression) => _context.Set<TAggregate>().Any(expression);


    public async Task<bool> ExistsAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default)
    => await _context.Set<TAggregate>().AnyAsync(expression, cancellationToken);

    #endregion

    #region Expression
    public IEnumerable<TAggregate> GetByExpression(Expression<Func<TAggregate, bool>> predicate, params Expression<Func<TAggregate, object>>[] joins)
        => _context.Set<TAggregate>().IncludeMany(joins).Where(predicate).ToList();


    public async Task<IList<TAggregate>> GetByExpressionAsync(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default, params Expression<Func<TAggregate, object>>[] joins)
    => await _context.Set<TAggregate>().IncludeMany(joins).Where(predicate).ToListAsync(cancellationToken);


    #endregion

    #region SaveChanges
    public bool SaveChanges()
    => _context.SaveChanges() > 0;

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    => await _context.SaveChangesAsync(cancellationToken) > 0;

    #endregion

    #region GetAll
    public IEnumerable<TAggregate> GetAll()
            => _context.Set<TAggregate>().ToList();


    public async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    => await _context.Set<TAggregate>().ToListAsync(cancellationToken);

    public IEnumerable<TAggregate> GetAllGraph()
    {
        IQueryable<TAggregate> query = GetGraphPaths();

        return query.ToList();


    }

    public async Task<IEnumerable<TAggregate>> GetAllGraphAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<TAggregate> query = GetGraphPaths();

        return await query.ToListAsync(cancellationToken);
    }
    #endregion

    #region Get
    #region Async
    public async Task<TAggregate> GetByAsync(TKey id, CancellationToken cancellationToken = default)
        => await _context.FindAsync<TAggregate>(id, cancellationToken);

    public async Task<TAggregate> GetGraphByAsync(TKey id, CancellationToken cancellationToken = default)
    {
        IQueryable<TAggregate> query = GetGraphPaths();

        return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }


    #endregion

    #region Sync

    public TAggregate? GetBy(TKey id) => _context.Find<TAggregate>(id);

    public TAggregate? GetGraphBy(TKey id)
    {
        IQueryable<TAggregate> query = GetGraphPaths();

        return query.FirstOrDefault(x => x.Id.Equals(id));
    }

    #endregion


    #endregion

    #region BusinessId
    public async Task<TAggregate> GetByAsync(BusinessId businessId, CancellationToken cancellationToken = default)
    => await _context.Set<TAggregate>().FirstOrDefaultAsync(x => x.BusinessId == businessId, cancellationToken);

    public async Task<TAggregate?> GetGraphByAsync(BusinessId businessId, CancellationToken cancellationToken = default)
    {
        IQueryable<TAggregate> query = GetGraphPaths();

        return await query.FirstOrDefaultAsync(x => x.BusinessId == businessId, cancellationToken);
    }



    public TAggregate? GetBy(BusinessId businessId)
    => _context.Set<TAggregate>().FirstOrDefault(x => x.BusinessId == businessId);

    public TAggregate? GetGraphBy(BusinessId businessId)
    {
        IQueryable<TAggregate> query = GetGraphPaths();

        return query.FirstOrDefault(x => x.BusinessId == businessId);
    }
    #endregion


}


