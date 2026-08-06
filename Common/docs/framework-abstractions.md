# Kootam.Framework.Abstractions

Shared query and entity marker interfaces used across the Kootam core framework.

| Property | Value |
|----------|-------|
| **NuGet** | `Kootam.Framework.Abstractions` |
| **Path** | `Common/Kootam.Framework.Abstractions/` |
| **Target** | `net10.0` |
| **Depends on** | `Kootam.Cqrs.Abstractions` |

Referenced by Application and query handlers that need pagination support.

---

## Types

### IHasId\<TKey\>

Marker for entities/DTOs exposed with an identifier:

```csharp
public interface IHasId<TKey>
{
    TKey Id { get; }
}
```

Use on DTOs returned from queries when generic constraints or shared logic require an `Id` property.

---

### IPageQuery\<T\> / PageQuery\<T\>

Paginated CQRS query contract extending `IQuery<T>`:

```csharp
public interface IPageQuery<T> : IQuery<T>
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
    int SkipCount => (PageNumber - 1) * PageSize;
    bool HasTotalCount { get; set; }
    string SortBy { get; set; }
}
```

Base class with defaults:

```csharp
public abstract class PageQuery<T> : IPageQuery<T>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int SkipCount => (PageNumber - 1) * PageSize;
    public bool HasTotalCount { get; set; }
    public string SortBy { get; set; } = "Id";
}
```

### PageData\<T\>

Standard paged result wrapper:

```csharp
public class PageData<T>
{
    public List<T> QueryResult { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
}
```

---

## Usage

### Define a paginated query

```csharp
using Kootam.Framework.Abstractions;
using Kootam.Cqrs.Abstractions.Queries;

public record GetProductListQuery : PageQuery<List<ProductDto>>
{
    public string? SearchTerm { get; init; }
}
```

### Handler

```csharp
public class GetProductListQueryHandler(IQueryRepository<Product, long> repo)
    : IQueryHandler<GetProductListQuery, PageData<ProductDto>>
{
    public async Task<Result<PageData<ProductDto>>> Handle(GetProductListQuery query, CancellationToken ct)
    {
        var all = await repo.GetAllAsync(ct);
        var filtered = string.IsNullOrEmpty(query.SearchTerm)
            ? all
            : all.Where(p => p.Name.Contains(query.SearchTerm));

        var total = filtered.Count();
        var page = filtered
            .Skip(query.SkipCount)
            .Take(query.PageSize)
            .Select(p => new ProductDto(p.Name))
            .ToList();

        return Result<PageData<ProductDto>>.Success(new PageData<ProductDto>
        {
            QueryResult = page,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = query.HasTotalCount ? total : 0
        });
    }
}
```

For dynamic grid filtering (sort, filter columns), use `IGridFilterQuery` from `Kootam.Framework.Application` instead.

---

## Agent Rules

- Paginated queries must extend `PageQuery<T>` or implement `IPageQuery<T>`.
- Return type for paginated handlers is typically `PageData<TDto>`, wrapped in `Result<PageData<TDto>>`.
- Set `HasTotalCount = true` when the client needs total record count for pagination UI.
- Default sort column is `"Id"` — override `SortBy` in the query or handler when needed.
- This package has no DI registration — reference it directly in Application project.
