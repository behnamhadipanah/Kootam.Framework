using Kootam.Framework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Kootam.Framework.Domain.Interfaces;


public interface IRepositoryBase<TEntity, TKey>
    where TEntity : AggregateRoot<TKey>
    where TKey : struct,IComparable,IComparable<TKey>,IConvertible,IEquatable<TKey>,IFormattable
{
    #region query by expression
    Task<IList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] joins);
    IEnumerable<TEntity> GetByExpression(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] joins);

    #endregion

    #region get all
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    IEnumerable<TEntity> GetAll();

    #endregion

    #region  get by id
    Task<TEntity> GetByAsync(long id, CancellationToken cancellationToken = default);
    TEntity GetBy(long id);
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
