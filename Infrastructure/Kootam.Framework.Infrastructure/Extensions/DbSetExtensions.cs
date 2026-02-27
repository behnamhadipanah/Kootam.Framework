using Kootam.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kootam.Framework.Infrastructure.Extensions;

public static class DbSetExtensions
{
    public static IQueryable<TEntity> Include<TEntity, TKey>
        (this DbSet<TEntity> query,
        Expression<Func<TEntity, object>>[] join)
        where TEntity : BaseEntity<TKey>
        where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable


    {
        IQueryable<TEntity> querable = query;
        if (join != null)
            join.ToList().ForEach(_join => { querable = querable.Include(_join); });
        return querable;
    }

    public static IQueryable<TEntity> Include<TEntity, TKey>(this IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] join) where TEntity : BaseEntity<TKey>
        where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        IQueryable<TEntity> querable = query;
        if (join != null)
            join.ToList().ForEach(_join => { querable = querable.Include(_join); });
        return querable;
    }
}

