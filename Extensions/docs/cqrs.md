# CQRS Extension

Command/Query Responsibility Segregation with FluentValidation, pipeline behaviors, and assembly-based handler registration.

| Package | Type | Path |
|---------|------|------|
| `Kootam.Cqrs.Abstractions` | Contracts | `Extensions/Kootam.Abstractions/Kootam.Cqrs.Abstractions/` |
| `Kootam.Cqrs` | Implementation | `Extensions/Cqrs/Kootam.Cqrs/src/Kootam.Cqrs/` |
| `Kootam.Cqrs.Sample` | Reference app | `Extensions/Cqrs/Kootam.Cqrs/src/Kootam.Cqrs.Sample/` |

---

## Registration

```csharp
using Kootam.Cqrs.DependencyInjections;

builder.Services.AddCqrs(options =>
{
    options.RegisterServicesFromAssemblyContaining<CreateProductCommandHandler>();
    options.RegisterServicesFromAssemblyContaining<GetProductListQueryHandler>();
    // or: options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

    options.EnableLogging = true;
    options.EnableValidation = true;
    options.EnableDomainExceptionHandling = true;
});
```

Registers:

- `IRequestDispatcher` → `RequestDispatcher`
- `IQueryDispatcher` → `QueryDispatcher`
- All `IRequestHandler<,>`, `IQueryHandler<,>` from registered assemblies
- All `IValidator<>` (FluentValidation) from registered assemblies
- Pipeline behaviors: `LoggingBehavior`, `ValidationBehavior`, `DomainExceptionBehavior`

---

## Commands

### Define

```csharp
public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;
```

### Handler

```csharp
public class CreateProductCommandHandler(
    ICommandRepository<Product, long> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var product = Product.Create(command.Name, command.Price);
        await repository.AddAsync(product, ct);
        await unitOfWork.CommitTransactionAsync(ct);
        return Result<Guid>.Success(product.BusinessId.Value);
    }
}
```

### Validator

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

Validation runs automatically via `ValidationBehavior` when `EnableValidation = true`.

### Dispatch (controller)

```csharp
public class ProductsController(IRequestDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await dispatcher.Send(command, HttpContext.RequestAborted);
        return Ok(result);
    }
}
```

Or use `BaseCqrsController` from `Kootam.Framework.Presentations`:

```csharp
public class ProductsController : BaseCqrsController
{
    [HttpPost]
    public Task<IActionResult> Create(CreateProductCommand cmd) => Create(cmd);
}
```

---

## Queries

### Define

```csharp
public record GetProductListQuery : IQuery<List<ProductDto>>;
```

### Handler

```csharp
public class GetProductListQueryHandler(IQueryRepository<Product, long> repo)
    : IQueryHandler<GetProductListQuery, List<ProductDto>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductListQuery query, CancellationToken ct)
    {
        var products = await repo.GetAllAsync(ct);
        var dtos = products.Select(p => new ProductDto(p.Name, p.Price)).ToList();
        return Result<List<ProductDto>>.Success(dtos);
    }
}
```

### Dispatch

```csharp
var result = await queryDispatcher.Execute(new GetProductListQuery(), ct);
```

---

## Result Mapping to HTTP

Use `Kootam.Framework.Presentations` extensions:

```csharp
return result.ToActionResult();       // Result
return result.ToActionResult<T>();    // Result<T>
```

Maps `ResultStatus` → HTTP status (200, 400, 404, 401, 403, 409, 500).

---

## File Organization (consuming service)

```
Application/
├── Commands/
│   └── Products/
│       ├── Create/
│       │   ├── CreateProductCommand.cs
│       │   ├── CreateProductCommandHandler.cs
│       │   └── CreateProductCommandValidator.cs
│       └── Update/
├── Queries/
│   └── Products/
│       ├── GetProductListQuery.cs
│       └── GetProductListQueryHandler.cs
```

Naming: `{Action}{Entity}Command`, `{Action}{Entity}Query`, `{Name}Handler`, `{Name}Validator`.

---

## Pipeline Behaviors

| Behavior | Purpose |
|----------|---------|
| `LoggingBehavior` | Logs request/handler execution |
| `ValidationBehavior` | Runs FluentValidation before handler |
| `DomainExceptionBehavior` | Catches domain exceptions, maps to `Result` |

---

## Agent Rules

- Every command with user input **must** have a FluentValidation validator.
- Handlers return `Result`/`Result<T>` — no exceptions for business rules.
- Register handler assemblies explicitly in `AddCqrs`.
- Keep commands/queries as `record` types in the Application project.
- Do not put EF Core or HTTP types in handlers when avoidable — inject abstractions.
