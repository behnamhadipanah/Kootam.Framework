# Kootam.Utilities.ScalarRegistration

Registers [Scalar](https://github.com/scalar/scalar) as an interactive OpenAPI documentation UI for ASP.NET Core APIs.

| Property | Value |
|----------|-------|
| **NuGet** | `Kootam.Utilities.ScalarRegistration` |
| **Path** | `Common/Kootam.Utilities.ScalarRegistration/` |
| **Target** | `net10.0` |
| **Depends on** | `Scalar.AspNetCore`, `Microsoft.AspNetCore.OpenApi` |
| **Sample** | `Kootam.Utilities.ScalarRegistration.Sample/` |

---

## Registration

### Inline options

```csharp
using Kootam.Utilities.ScalarRegistration.DependencyInjection;

builder.Services.AddScalar(options =>
{
    options.Enabled = true;
    options.Name = "Product API";
    options.Title = "Product Service";
    options.Version = "1.0.0";
    options.Description = "Product management endpoints";
});
```

### From configuration

**appsettings.json:**

```json
{
  "Scalar": {
    "Enabled": true,
    "Name": "Product API",
    "Title": "Product Service",
    "Version": "1.0.0",
    "Description": "Product management endpoints"
  }
}
```

```csharp
builder.Services.AddScalar(builder.Configuration, "Scalar");
// or bind root section:
builder.Services.AddScalar(builder.Configuration);
```

### Default (enabled)

```csharp
builder.Services.AddScalar();  // Enabled = true by default
```

When `Enabled = true`, `AddScalar` also calls `AddOpenApi()` internally.

---

## Middleware

```csharp
var app = builder.Build();

app.UseScalar();  // Maps OpenAPI + Scalar UI when Enabled
```

`UseScalar()` reads `ScalarOption` from DI and:

1. Calls `app.MapOpenApi()` if enabled
2. Calls `app.MapScalarApiReference()` for the Scalar UI

---

## ScalarOption

| Property | Default | Description |
|----------|---------|-------------|
| `Enabled` | `true` | Toggle Scalar + OpenAPI |
| `Name` | — | API name shown in Scalar |
| `Title` | — | Document title |
| `Version` | — | API version string |
| `Description` | `""` | API description |

---

## Full Example

```csharp
using Kootam.Utilities.ScalarRegistration.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScalar(options =>
{
    options.Enabled = true;
    options.Name = "My API";
    options.Title = "My Service";
    options.Version = "1.0.0";
    options.Description = "Backend API documentation";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseScalar();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

Access Scalar UI at the route mapped by `MapScalarApiReference()` (typically `/scalar/v1`).

---

## Development vs Production

Restrict Scalar to development environments:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseScalar();
}
```

Do not expose interactive API docs in production unless explicitly required.

---

## Agent Rules

- Call `AddScalar()` in `Program.cs` service registration — not in individual controllers.
- Call `UseScalar()` before or after `MapControllers()` — both work; keep with other dev-only middleware.
- Set `Enabled = false` in production `appsettings.Production.json` if the section is bound from config.
- Scalar requires OpenAPI — do not remove `AddOpenApi()` when using this package with `Enabled = true`.
- Reference sample: `Common/Kootam.Utilities.ScalarRegistration/Kootam.Utilities.ScalarRegistration.Sample/Program.cs`
