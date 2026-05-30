using Kootam.Framework.Domain.Entities;
using Kootam.Framework.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Kootam.Framework.Domain.Contracts;


public interface ICommandRepository<TEntity, TKey>
    where TEntity : AggregateRoot<TKey>
    where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
{
    #region query by expression
    Task<IList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] joins);
    IEnumerable<TEntity> GetByExpression(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] joins);

    #endregion

    #region get all
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    IEnumerable<TEntity> GetAll();
    IEnumerable<TEntity> GetAllGraph();
    Task<IEnumerable<TEntity>> GetAllGraphAsync(CancellationToken cancellationToken = default);

    #endregion

    #region  get by id
    Task<TEntity> GetByAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TEntity> GetGraphByAsync(TKey id, CancellationToken cancellationToken = default);
    TEntity? GetBy(TKey id);
    TEntity? GetGraphBy(TKey id);
    #endregion

    #region businessId
    Task<TEntity> GetByAsync(BusinessId businessId, CancellationToken cancellationToken = default);
    Task<TEntity> GetGraphByAsync(BusinessId businessId, CancellationToken cancellationToken = default);
    TEntity GetBy(BusinessId businessId);
    TEntity? GetGraphBy(BusinessId businessId);
    #endregion

    #region add
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    #endregion


    #region exists
    bool Exists(Expression<Func<TEntity, bool>> expression);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
    #endregion

    #region save changes
    bool SaveChanges();
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    #endregion

}
