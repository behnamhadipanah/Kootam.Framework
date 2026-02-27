using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kootam.Framework.Application.Queries.Grid;

public interface IGridFilterQuery
{
    Task<GridFilterResponse<TDto>> ApplyAsync<TEntity, TDto>(
         IQueryable<TEntity> query,
   GridFilter request,
   Expression<Func<TEntity, TDto>> selector,
   CancellationToken cancellationToken = default) where TEntity : class;
}


public sealed class GridFilterQuery : IGridFilterQuery
{
    public async Task<GridFilterResponse<TDto>> ApplyAsync<TEntity, TDto>(
        IQueryable<TEntity> source,
        GridFilter request,
        Expression<Func<TEntity, TDto>> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var filteredQuery = ApplyFilters(source, request.Filters);

        var total = await filteredQuery.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SortColumn))
        {
            filteredQuery = ApplySorting(filteredQuery, request);
        }

        var data = await filteredQuery
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new GridFilterResponse<TDto>(data, total);
    }

    private static IQueryable<TEntity> ApplySorting<TEntity>(
        IQueryable<TEntity> query,
        GridFilter request)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.PropertyOrField(parameter, request.SortColumn!);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = request.SortDirection == SortDirection.Desc
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        var call = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(TEntity), property.Type },
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<TEntity>(call);
    }

    private static IQueryable<TEntity> ApplyFilters<TEntity>(
        IQueryable<TEntity> query,
        IReadOnlyDictionary<string, string>? filters)
        where TEntity : class
    {
        if (filters == null || filters.Count == 0)
            return query;

        foreach (var (propertyName, rawValue) in filters)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                continue;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.PropertyOrField(parameter, propertyName);

            var predicate = BuildPredicate<TEntity>(property, parameter, rawValue);
            if (predicate is null)
                continue;

            query = query.Where(predicate);
        }

        return query;
    }

    private static Expression<Func<TEntity, bool>>? BuildPredicate<TEntity>(
        MemberExpression property,
        ParameterExpression parameter,
        string value)
    {
        var type = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

        Expression body;

        if (type == typeof(string))
        {
            body = BuildStringPredicate(property, value);
        }
        else if (type == typeof(bool))
        {
            if (!bool.TryParse(value, out var boolValue))
                return null;

            body = Expression.Equal(property, Expression.Constant(boolValue));
        }
        else if (type.IsPrimitive || type == typeof(decimal))
        {
            try
            {
                var converted = Convert.ChangeType(value, type);
                body = Expression.Equal(property, Expression.Constant(converted));
            }
            catch
            {
                return null;
            }
        }
        else
        {
            body = BuildStringPredicate(
                Expression.Call(property, nameof(ToString), null),
                value);
        }

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private static Expression BuildStringPredicate(Expression property, string value)
    {
        var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        var left = Expression.Call(property, toLower);
        var right = Expression.Constant(value.ToLower());

        return Expression.Call(left, contains, right);
    }
}

