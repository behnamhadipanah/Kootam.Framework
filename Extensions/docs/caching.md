# Caching Extension

Pluggable caching with a unified `ICacheStore` abstraction. Three backends: InMemory, Redis, SQL.

| Package | Backend | Sample |
|---------|---------|--------|
| `Kootam.Caching.Abstractions` | Contracts | — |
| `Kootam.Caching.InMemory` | `MemoryCache` | `Extensions/Chaching/Kootam.Caching.InMemory/src/Kootam.Caching.InMemory.Sample/` |
| `Kootam.Caching.Redis` | Redis (StackExchange) | `Extensions/Chaching/Kootam.Caching.Redis/src/Kootam.Caching.Redis.Sample/` |
| `Kootam.Caching.Sql` | SQL Server | `Extensions/Chaching/Kootam.Caching.Sql/src/Kootam.Caching.Sql.Sample/` |

> Folder is named `Chaching` (typo preserved in repo).

---

## Abstraction

```csharp
public interface ICacheStore : ICacheReader, ICacheWriter
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<bool> ExistsAsync(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task<bool> RemoveAsync(string key);
}
```

Inject `ICacheStore` in query handlers or application services.

---

## InMemory (Development / Single Instance)

```csharp
using Kootam.Caching.InMemory.DependencyInjection;

builder.Services.AddInMemoryCaching();
```

- **Lifetime:** Singleton
- **Use when:** Local dev, unit tests, single-node apps without shared state

---

## Redis (Production / Distributed)

### Via configuration

```csharp
using Kootam.Caching.Redis.DependencyInjection;

builder.Services
    .AddRedisCaching(builder.Configuration)
    .AddRedisHealthChecks();
```

**appsettings.json:**

```json
{
  "RedisDBConfigs": {
    "Configs": [
      {
        "Name": "Default",
        "Host": "localhost",
        "Port": 6379,
        "DBNumber": 0,
        "Password": null
      }
    ]
  }
}
```

### Inline configuration

```csharp
builder.Services.AddRedisCaching(options =>
{
    options.Configs = new List<RedisDBConfigModel>
    {
        new() { Name = "Default", Host = "localhost", Port = 6379, DBNumber = 0 }
    };
});

// Shorthand
builder.Services.AddRedisCaching(
    host: "localhost",
    port: 6379,
    database: 0,
    password: null,
    configName: "Default");
```

### Health checks

```csharp
builder.Services.AddRedisHealthChecks();  // registers /health check "redis"
app.MapHealthChecks("/health");
```

---

## SQL (Persistent Cache)

Package: `Kootam.Caching.Sql` — use when Redis is unavailable and DB-backed cache is acceptable.

Register via the package's `ServiceCollection` extension (see sample project). Sample is a scaffold; configure connection string in `appsettings.json`.

---

## Usage in Handlers

```csharp
public class GetProductByIdQueryHandler(ICacheStore cache, IQueryRepository<Product, long> repo)
    : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var cacheKey = $"product:{query.Id}";

        var cached = await cache.GetAsync<ProductDto>(cacheKey);
        if (cached is not null)
            return Result<ProductDto>.Success(cached);

        var product = await repo.GetByAsync(query.Id, ct);
        if (product is null)
            return Result<ProductDto>.Failure(ResultStatus.NotFound, "Product not found");

        var dto = new ProductDto(product.Name, product.Price);
        await cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return Result<ProductDto>.Success(dto);
    }
}
```

---

## Cache Key Conventions

Use namespaced keys to avoid collisions:

```
{service}:{entity}:{id}          → product:123
{service}:{entity}:list:{hash}   → product:list:abc123
```

Always set expiry for data that can become stale.

---

## Agent Rules

- Register **one** cache backend per service — do not register InMemory and Redis together.
- Application layer depends on `ICacheStore` only.
- Always pass `TimeSpan? expiry` on `SetAsync` for non-static data.
- Use `AddRedisHealthChecks()` in production Redis deployments.
- Invalidate cache in command handlers after writes (`RemoveAsync` or pattern removal).
