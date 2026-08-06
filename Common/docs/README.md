# Kootam Common — Documentation

Reference for **backend developers** and **AI agents** using shared Kootam class libraries from the `Common/` folder.

These packages provide cross-cutting abstractions, API response envelopes, utility extensions, and observability setup used by the core framework and consuming backend services.

---

## Package Map

| NuGet Package | Purpose | Doc |
|---------------|---------|-----|
| `Kootam.Framework.Abstractions` | Paging queries, `IHasId` marker | [framework-abstractions.md](./framework-abstractions.md) |
| `Kootam.Framework.Utilities` | `ApiResponse`, exceptions, string/date/file helpers | [framework-utilities.md](./framework-utilities.md) |
| `Kootam.Utilities.SerilogRegistration` | Serilog enrichers & bootstrap | [serilog-registration.md](./serilog-registration.md) |
| `Kootam.Utilities.ScalarRegistration` | Scalar OpenAPI UI | [scalar-registration.md](./scalar-registration.md) |
| `Kootam.Framework.Utilities.Tools` | Legacy duplicate of Utilities (avoid) | [framework-utilities.md](./framework-utilities.md#utilities-vs-utilitiestools) |

---

## Folder Layout

```
Common/
├── Kootam.Framework.Abstractions/          # Shared query/entity markers
├── Kootam.Framework.Utilities.Common/      # → NuGet: Kootam.Framework.Utilities
├── Kootam.Framework.Utilities.Tools/       # Legacy (do not use in new projects)
├── Kootam.Utilities.SerilogRegistration/   # Structured logging enrichers
├── Kootam.Utilities.ScalarRegistration/    # API documentation UI
└── docs/                                   # This documentation
```

---

## Which Package to Reference

| Layer / Need | Package |
|--------------|---------|
| Paginated CQRS queries | `Kootam.Framework.Abstractions` |
| Standard API response envelope | `Kootam.Framework.Utilities` |
| Persian/Arabic string normalization | `Kootam.Framework.Utilities` |
| Domain/business exceptions | `Kootam.Framework.Utilities` |
| Structured logging with user/app context | `Kootam.Utilities.SerilogRegistration` |
| Interactive API docs (Scalar) | `Kootam.Utilities.ScalarRegistration` |

---

## Core Framework Dependencies

These Common packages are referenced by the main framework:

| Consumer | References |
|----------|------------|
| `Kootam.Framework.Infrastructure` | `Kootam.Framework.Utilities` |
| `Kootam.Framework.Presentations` | `Kootam.Framework.Utilities` |
| `Kootam.Framework.Abstractions` (core) | Used alongside CQRS abstractions |

When building a new backend service on Kootam, reference the same packages the framework uses.

---

## Quick Examples

### Paginated query

```csharp
public record GetProductListQuery : PageQuery<List<ProductDto>>;
```

### API response

```csharp
return ApiResponse<ProductDto>.Ok(product);
return ApiResponse.Fail(StatusCodes.Status404NotFound, "Not found");
```

### Scalar (dev API docs)

```csharp
builder.Services.AddScalar(options => { options.Enabled = true; });
// ...
app.UseScalar();
```

---

## Agent Guidelines

- Prefer **`Kootam.Framework.Utilities`** over `Kootam.Framework.Utilities.Tools` — Tools is a legacy duplicate with broken references.
- Use `ApiResponse<T>` for all HTTP responses — do not create custom response wrappers.
- Use framework exceptions (`NotExistsException`, `DuplicatedRecordException`) for known error cases in repositories/services.
- `Kootam.Framework.Abstractions` depends on `Kootam.Cqrs.Abstractions` — paging queries must implement `IQuery<T>`.
- Serilog enrichers require `IUserInfoService` for user context — register auth/user management first.

---

## Related

- [Root README](../../README.md) — Core framework architecture
- [Extensions/docs](../Extensions/docs/README.md) — Optional extension packages
- Pack script: `script/win/Pack-Kootam.Framework.ps1`
