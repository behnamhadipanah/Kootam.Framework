# AutoMap Extension

Abstraction over object-mapping libraries. Swap AutoMapper or Mapster without changing Application code.

| Package | Library | Sample |
|---------|---------|--------|
| `Kootam.AutoMap.Abstractions` | `IMapper` contract | — |
| `Kootam.AutoMap.AutoMapper` | AutoMapper | `Extensions/AutoMap/Kootam.AutoMap.AutoMapper/src/Kootam.AutoMap.AutoMapper.Sample/` |
| `Kootam.AutoMap.Mapster` | Mapster | `Extensions/AutoMap/Kootam.AutoMap.Mapster/src/Kootam.AutoMap.Mapster.Sample/` |

---

## Abstraction

```csharp
public interface IMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);
}
```

Inject `IMapper` in query handlers and application services — never inject AutoMapper/Mapster types directly.

---

## Mapster (Recommended for Performance)

```csharp
using Kootam.AutoMap.Mapster.DependencyInjection;

builder.Services.AddMapsterMapping();
```

Define mapping profiles using Mapster's `TypeAdapterConfig` or attribute-based mapping in your Application/Infrastructure project.

---

## AutoMapper

```csharp
using Kootam.AutoMap.AutoMapper.DependencyInjection;

builder.Services.AddAutoMapperMapping(
    typeof(ProductProfile).Assembly,
    typeof(OrderProfile).Assembly);
```

Create `Profile` classes in Application or Infrastructure:

```csharp
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductCommand, Product>();
    }
}
```

---

## Usage in Query Handlers

```csharp
public class GetProductListQueryHandler(IQueryRepository<Product, long> repo, IMapper mapper)
    : IQueryHandler<GetProductListQuery, List<ProductDto>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductListQuery query, CancellationToken ct)
    {
        var products = await repo.GetAllAsync(ct);
        var dtos = products.Select(p => mapper.Map<ProductDto>(p)).ToList();
        return Result<List<ProductDto>>.Success(dtos);
    }
}
```

### EF Core projection (read queries)

```csharp
var dtos = mapper.ProjectTo<ProductDto>(context.Products.AsQueryable()).ToList();
```

Prefer `ProjectTo` for list queries — maps at SQL level.

---

## Choosing a Backend

| Criteria | Mapster | AutoMapper |
|----------|---------|------------|
| Performance | Faster (compile-time) | Good |
| Learning curve | Lower | Higher (profiles) |
| Ecosystem | Smaller | Larger |
| EF projection | Supported | Supported |

Pick **one** per service. Do not register both.

---

## Agent Rules

- Map in Application layer (handlers) or dedicated mapping profiles — not in controllers.
- Use `IMapper` abstraction — no direct `Mapper.Map` from AutoMapper in handlers.
- Register mapping assemblies explicitly in `AddAutoMapperMapping(...)`.
- For simple 1:1 DTOs with few fields, manual mapping is acceptable — do not add mapping infrastructure for trivial cases.
