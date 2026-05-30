using System.Linq.Expressions;
using Kootam.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kootam.Framework.Infrastructure.Extensions.FluentInclude;

/// <summary>
/// Provides a fluent builder for adding Include statements to an EF Core query.
/// This class allows navigation properties to be included in a readable
/// chained syntax and returns the final query through the Query property.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type being queried.
/// </typeparam>
/// <example>
/// <code>
/// var orders = context.Orders
///     .IncludeFluent()
///         .Include(o => o.Customer)
///         .Include(o => o.Items)
///     .Query;
/// </code>
/// </example>
public sealed class IncludeBuilder<TEntity> where TEntity:class
{
    private IQueryable<TEntity> _query;
    /// <summary>
    /// Initializes a new instance of the IncludeBuilder class.
    /// </summary>
    /// <param name="query">
    /// The base query that includes will be applied to.
    /// </param>
    internal IncludeBuilder(IQueryable<TEntity> query)
    {
        _query = query;
    }
    /// <summary>
    /// Includes the specified navigation property in the query.
    /// </summary>
    /// <typeparam name="TProperty">
    /// The type of the navigation property.
    /// </typeparam>
    /// <param name="navigation">
    /// An expression representing the navigation property to include.
    /// </param>
    /// <returns>
    /// The current IncludeBuilder instance for chaining additional includes.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = context.Orders
    ///     .IncludeFluent()
    ///         .Include(o => o.Customer)
    ///     .Query;
    /// </code>
    /// </example>
    public IncludeBuilder<TEntity> Include<TProperty>(
      Expression<Func<TEntity, TProperty>> navigation)
    {
        _query = _query.Include(navigation);
        return this;
    }

    /// <summary>
    /// Gets the resulting query after all Include operations have been applied.
    /// </summary>
    /// <example>
    /// <code>
    /// var query = context.Orders
    ///     .IncludeFluent()
    ///         .Include(o => o.Customer)
    ///     .Query;
    ///
    /// var list = await query.ToListAsync();
    /// </code>
    /// </example>
    public IQueryable<TEntity> Build()
         => _query;
}
