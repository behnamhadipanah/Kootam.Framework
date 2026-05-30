using Kootam.Framework.Infrastructure.Extensions.FluentInclude;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kootam.Framework.Infrastructure.Extensions;

/// <summary>
/// Provides helper extension methods for applying Entity Framework Core
/// Include operations in a cleaner and more reusable way.
/// 
/// Supports both:
/// - Simple multiple Includes (IncludeMany)
/// - Fluent Include builder pattern (IncludeFluent)
/// </summary>
///     /// <example>
/// Simple multiple include:
/// <code>
/// var orders = context.Orders
///     .IncludeMany(o => o.Customer, o => o.Items);
/// </code>
///
/// Fluent include:
/// <code>
/// var orders = context.Orders
///     .IncludeFluent()
///         .Include(o => o.Customer)
///         .Include(o => o.Items)
///     .Query;
/// </code>
/// </example>
public static class QueryableIncludeExtensions
{
    /// <summary>
    /// Applies multiple Include expressions to the query.
    /// This method simplifies adding several navigation property includes
    /// without chaining multiple Include calls manually.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type being queried.
    /// </typeparam>
    /// <param name="query">
    /// The source query to apply includes to.
    /// </param>
    /// <param name="includes">
    /// One or more navigation property expressions to include.
    /// </param>
    /// <returns>
    /// The query with all specified navigation properties included.
    /// </returns>
    /// <example>
    /// <code>
    /// var users = context.Users
    ///     .IncludeMany(u => u.Roles, u => u.Profile);
    /// </code>
    /// </example>
    public static IQueryable<TEntity> IncludeMany<TEntity>(
        this IQueryable<TEntity> query,
        params Expression<Func<TEntity, object>>[] includes)
        where TEntity : class
        
    {
        if (includes == null || includes.Length == 0)
            return query;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
    
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
        this IQueryable<TEntity> query) where TEntity : class
    {
        return new IncludeBuilder<TEntity>(query);
    }
}