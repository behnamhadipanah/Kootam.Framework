using Kootam.Framework.Domain.Entities;

namespace Kootam.Framework.Infrastructure.Extensions.FluentInclude;

public static class FluentIncludeExtensions
{
    /// <summary>
    /// Starts a fluent include builder for the given query.
    /// This allows chaining multiple Include calls in a fluent style
    /// and retrieving the final query using the Query property.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type being queried.
    /// </typeparam>
    /// <param name="query">
    /// The source query to build includes on.
    /// </param>
    /// <returns>
    /// An IncludeBuilder used to fluently define Include operations.
    /// </returns>
    /// <example>
    /// <code>
    /// var orders = context.Orders
    ///     .IncludeFluent()
    ///         .Include(o => o.Customer)
    ///         .Include(o => o.Items)
    ///     .Query;
    /// </code>
    /// </example>
    public static IncludeBuilder<TEntity> IncludeFluent<TEntity>(
        this IQueryable<TEntity> query) where TEntity:BaseEntity
    {
        return new IncludeBuilder<TEntity>(query);
    }
}