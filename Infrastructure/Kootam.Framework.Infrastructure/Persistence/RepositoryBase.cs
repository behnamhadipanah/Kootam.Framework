using Kootam.Framework.Domain.Entities;
using Kootam.Framework.Domain.Interfaces;
using Kootam.Framework.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Kootam.Framework.Infrastructure.Persistence;

public class RepositoryBase<TEntity, TDbContext, TKey> : IRepositoryBase<TEntity, TKey>
    where TEntity : AggregateRoot<TKey>
    where TDbContext : BaseCommandDbContext
     where TKey : struct,
          IComparable,
          IComparable<TKey>,
          IConvertible,
          IEquatable<TKey>,
          IFormattable
{
    private readonly TDbContext _context;
    public RepositoryBase(TDbContext context)
    {
        _context = context;
    }

    public void Add(TEntity entity)
    {
        _context.Add<TEntity>(entity);

    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync<TEntity>(entity,cancellationToken);

    }


    public bool Exists(Expression<Func<TEntity, bool>> expression)
    {
        return _context.Set<TEntity>().Any(expression);

    }
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().AnyAsync(expression,cancellationToken);

    }

    public IEnumerable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().ToListAsync(cancellationToken);

    }

    public async Task<TEntity> GetByAsync(long id, CancellationToken cancellationToken = default)
    {

        return await _context.FindAsync<TEntity>(id,cancellationToken);

    }

    public TEntity GetBy(long id)
    {
        return _context.Find<TEntity>(id);

    }

    public IEnumerable<TEntity> GetByExpression(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] joins)
    {
        return _context.Set<TEntity>().Include<TEntity, TKey>(joins).Where(predicate).ToList();

    }

    public async Task<IList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] joins)
    {
        return await _context.Set<TEntity>().Include<TEntity, TKey>(joins).Where(predicate).ToListAsync(cancellationToken);

    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

}