# Kootam.Utilities.SerilogRegistration

Serilog enrichers and bootstrap helper for structured logging with application and user context.

| Property | Value |
|----------|-------|
| **NuGet** | `Kootam.Utilities.SerilogRegistration` |
| **Path** | `Common/Kootam.Utilities.SerilogRegistration/Kootam.Utilities.SerilogRegistration/` |
| **Target** | `net10.0` |
| **Depends on** | Serilog, `Kootam.UserManagement.Abstractions` |

---

## Components

| Type | Purpose |
|------|---------|
| `SerilogExtensions` | Bootstrap wrapper with fatal exception handling |
| `ApplicationEnricher` | Adds app/service metadata to every log event |
| `UserInfoEnricher` | Adds current user info to every log event |
| `SerilogApplicationEnricherOptions` | Configuration for application enricher |

---

## Bootstrap Helper

Wraps application startup with Serilog lifecycle management:

```csharp
using Kootam.Utilities.SerilogRegistration.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

SerilogExtensions.RunWithSerilogExceptionHandling(() =>
{
    var app = WebApplication.CreateBuilder(args).Build();
    app.Run();
},
startUpMessage: "Starting MyService",
exceptionMessage: "Unhandled exception",
shutdownMessage: "Shutdown complete");
```

Catches unhandled exceptions, logs fatal, and flushes on shutdown.

---

## Application Enricher

Adds these properties to every log event:

| Property | Source |
|----------|--------|
| `ApplicationName` | `SerilogApplicationEnricherOptions` |
| `ServiceName` | Options |
| `ServiceVersion` | Options |
| `ServiceId` | Options |
| `MachineName` | `Environment.MachineName` |
| `EntryPoint` | Entry assembly name |

### Configuration

```csharp
builder.Services.Configure<SerilogApplicationEnricherOptions>(options =>
{
    options.ApplicationName = "Kootam";
    options.ServiceName = "ProductService";
    options.ServiceVersion = "1.0.0";
    options.ServiceId = "product-api";
});
```

Register in Serilog pipeline:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.With<ApplicationEnricher>()
    .WriteTo.Console()
    .CreateLogger();
```

Or via DI-enriched configuration in `Program.cs` using `ReadFrom.Services(services)`.

---

## User Info Enricher

Adds authenticated user context to logs. Requires `IUserInfoService` registered in DI.

| Property | Source |
|----------|--------|
| `UserName` | `IUserInfoService.GetUsername()` |
| `UserId` | `IUserInfoService.UserIdOrDefault()` |
| `UserIp` | `IUserInfoService.GetUserIp()` |
| `ClientId` | Claim `"client_id"` or `"Unknown"` |

```csharp
builder.Services.AddScoped<IUserInfoService, AppUserInfoService>();

Log.Logger = new LoggerConfiguration()
    .Enrich.With<UserInfoEnricher>()
    .WriteTo.Console()
    .CreateLogger();
```

Register auth and `IUserInfoService` **before** configuring Serilog when using `ReadFrom.Services`.

---

## Full Program.cs Example

```csharp
using Kootam.Utilities.SerilogRegistration.Enrichers;
using Kootam.Utilities.SerilogRegistration.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SerilogApplicationEnricherOptions>(options =>
{
    options.ApplicationName = "Kootam";
    options.ServiceName = "OrderService";
    options.ServiceVersion = "1.0.0";
    options.ServiceId = "order-api";
});

builder.Services.AddScoped<IUserInfoService, AppUserInfoService>();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.With<ApplicationEnricher>()
        .Enrich.With<UserInfoEnricher>()
        .WriteTo.Console();
});

var app = builder.Build();
app.Run();
```

---

## Dependencies

The package references:

- `Serilog` 4.x
- `Serilog.Enrichers.Span` — distributed tracing
- `Serilog.Exceptions` — structured exception details

Configure sinks and levels in `appsettings.json` under `"Serilog"` section.

---

## Agent Rules

- Register `IUserInfoService` before Serilog reads from DI services.
- Use `ApplicationEnricher` in all production services for consistent log metadata.
- Use `UserInfoEnricher` when authentication is enabled — omit in anonymous/public APIs.
- Do not log sensitive data (passwords, tokens) even with enrichers enabled.
- Prefer `builder.Host.UseSerilog(...)` over manual `Log.Logger` assignment in ASP.NET Core apps.
