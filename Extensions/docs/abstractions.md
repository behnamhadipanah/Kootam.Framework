# Kootam.Abstractions

Shared contracts used across all Kootam extensions. These are **class libraries with interfaces only** — no DI registration, no runtime behavior.

**Location:** `Extensions/Kootam.Abstractions/`

---

## Packages

| NuGet Package | Purpose |
|---------------|---------|
| `Kootam.Cqrs.Abstractions` | Commands, queries, handlers, `Result<T>`, pipeline behaviors |
| `Kootam.Authentication.Abstractions` | Auth services, tokens, claims, current user |
| `Kootam.Caching.Abstractions` | Cache read/write/store interfaces |
| `Kootam.AutoMap.Abstractions` | `IMapper` abstraction |
| `Kootam.MessageBroker.Abstractions` | Publisher, subscriber, message envelope |
| `Kootam.Translator.Abstractions` | `ITranslator`, `ITranslationStore` |
| `Kootam.UserManagement.Abstractions` | `IUserInfoService` |
| `Kootam.DependencyInjection.Abstractions` | Lifetime marker interfaces |
| `Kootam.Serializers.Abstractions` | `IExcelSerializer` (WIP) |

---

## CQRS Abstractions

### Commands

```csharp
public record CreateOrderCommand(string CustomerName) : IRequest<Guid>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken ct) { ... }
}
```

| Type | Role |
|------|------|
| `IRequest` / `IRequest<TResult>` | Command marker |
| `IRequestHandler<TRequest>` | Handler returning `Result` |
| `IRequestHandler<TRequest, TResult>` | Handler returning `Result<TResult>` |
| `IRequestDispatcher` | Sends commands (`Send`, `Send<TResult>`) |
| `IQuery<TResult>` | Query marker |
| `IQueryHandler<TQuery, TResult>` | Query handler |
| `IQueryDispatcher` | Executes queries (`Execute<TResult>`) |
| `Result` / `Result<T>` | Operation outcome with `ResultStatus` |
| `IPipelineBehavior<TRequest, TResponse>` | Cross-cutting pipeline (logging, validation) |

### ResultStatus values

`Success`, `ValidationError`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`, `Error`

Handlers must return `Result`/`Result<T>` — never throw for expected business failures.

---

## Authentication Abstractions

| Type | Role |
|------|------|
| `IAuthenticationService<TUserKey>` | Sign in / sign out |
| `IPasswordHasherService` | Password hashing |
| `IRefreshTokenService<TUserKey>` | Refresh token lifecycle |
| `ITokenGenerator<TUser, TUserKey>` | Issue access/refresh tokens |
| `ITokenValidator` | Validate access tokens |
| `IAccessTokenReader` / `IRefreshTokenReader` | Read tokens from transport (cookie, header) |
| `ITokenStore<TUserKey>` | Write tokens to transport |
| `ICurrentUserAccessor` | Access current authenticated user |
| `IUserClaimsMapper<TUser>` | Map domain user to claims |

---

## Caching Abstractions

```csharp
public interface ICacheStore : ICacheReader, ICacheWriter { }

public interface ICacheReader
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<bool> ExistsAsync(string key);
}

public interface ICacheWriter
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task<bool> RemoveAsync(string key);
}
```

Additional interfaces: `ICacheExpiration`, `ICacheGetOrRefresh`, `ICachePatternRemoval`.

Inject `ICacheStore` in handlers/services — never reference Redis or InMemory types directly in Application layer.

---

## Message Broker Abstractions

| Type | Role |
|------|------|
| `IMessagePublisher` | Publish messages to a queue |
| `IMessageSubscriber` | Subscribe with a handler delegate |
| `IMessageBrokerConnection` | Connection lifecycle |
| `MessageEnvelope<T>` | Payload + metadata wrapper |
| `MessageContext<T>` | Received message + metadata |
| `MessageMetadata` | CorrelationId, headers, source |
| `SubscribeOptions` | PrefetchCount, AutoAck, Durable |
| `PublishOptions` | Publish configuration |

---

## AutoMap Abstractions

```csharp
public interface IMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);
}
```

---

## Translator Abstractions

```csharp
public interface ITranslator
{
    string Get(string key);
    string Get(string key, params object[] arguments);
    string Get(string key, CultureInfo culture);
}
```

Indexer syntax: `translator["WelcomeMessage"]`

---

## User Management Abstractions

```csharp
public interface IUserInfoService
{
    string UserId();
    string GetUsername();
    string GetFirstName();
    string GetLastName();
    string GetUserIp();
    string GetUserAgent();
    string? GetClaim(string claimType);
    bool HasAccess(string claimType, string value);
}
```

`FakeUserInfoService` is available for development/testing.

---

## Dependency Injection Markers

| Interface | Lifetime |
|-----------|----------|
| `ISingletonLifetime` | Singleton |
| `IScopeLifetime` | Scoped |
| `ITransientLifetime` | Transient |

Use for convention-based assembly scanning in consuming services.

---

## For AI Agents

- Application projects should reference **abstraction** packages only.
- Infrastructure / API `Program.cs` registers the concrete implementation.
- Do not duplicate these interfaces in consuming services.
- See implementation docs: [cqrs.md](./cqrs.md), [authentication.md](./authentication.md), etc.
