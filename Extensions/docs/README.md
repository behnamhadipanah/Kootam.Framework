# Kootam Extensions — Documentation

Reference for **backend developers** and **AI agents** implementing services with Kootam extension class libraries.

Each extension is a standalone NuGet package under `Extensions/`. Abstractions define contracts; implementation packages provide DI registration and adapters. Sample projects (`*.Sample`) demonstrate real usage.

---

## Package Map

| Area | Abstraction | Implementations | Doc |
|------|-------------|-----------------|-----|
| CQRS | `Kootam.Cqrs.Abstractions` | `Kootam.Cqrs` | [cqrs.md](./cqrs.md) |
| Authentication | `Kootam.Authentication.Abstractions` | `Kootam.Authentication`, `.Jwt`, `.SqlServer`, `.Redis`, `.InMemory` | [authentication.md](./authentication.md) |
| Caching | `Kootam.Caching.Abstractions` | `Kootam.Caching.InMemory`, `.Redis`, `.Sql` | [caching.md](./caching.md) |
| Auto Mapping | `Kootam.AutoMap.Abstractions` | `Kootam.AutoMap.AutoMapper`, `Kootam.AutoMap.Mapster` | [automap.md](./automap.md) |
| Message Broker | `Kootam.MessageBroker.Abstractions` | `Kootam.MessageBroker.RabbitMQ`, `.Kafka`, `.AzureServiceBus` | [message-broker.md](./message-broker.md) |
| Translation | `Kootam.Translator.Abstractions` | `Kootam.Translator.Json`, `Kootam.Translator.Database` | [translator.md](./translator.md) |
| User Management | `Kootam.UserManagement.Abstractions` | `Kootam.UserManagement` | [user-management.md](./user-management.md) |
| Shared contracts | `Kootam.Abstractions/*` | — | [abstractions.md](./abstractions.md) |

---

## Folder Layout

```
Extensions/
├── Kootam.Abstractions/     # Shared interfaces (no implementation)
├── Cqrs/
├── Authentication/
├── Chaching/                # Note: folder name uses "Chaching"
├── AutoMap/
├── MessageBroker/
├── Translator/
├── UserManagement/
├── Serializers/             # Work in progress
└── docs/                    # This documentation
```

Each extension follows a common structure:

```
{ExtensionName}/
├── src/
│   ├── {PackageName}/           # Class library
│   └── {PackageName}.Sample/    # Reference host app
├── {PackageName}.sln
└── pack.ps1
```

---

## Quick Start (Any Extension)

1. **Pack** extensions locally (if not on a remote feed):

   ```powershell
   .\script\win\Pack-Extensions.ps1
   ```

2. **Reference** the abstraction + one implementation in your service project.

3. **Register** via the extension's `Add*` / `Use*` method in `Program.cs`.

4. **Consult** the matching `*.Sample` project before writing new integration code.

---

## Choosing an Implementation

| Need | Recommended package |
|------|---------------------|
| Commands & queries with validation | `Kootam.Cqrs` |
| Cookie / header JWT auth | `Kootam.Authentication` + `Kootam.Authentication.Jwt` |
| Refresh tokens in SQL Server | `Kootam.Authentication.SqlServer` |
| Refresh tokens in Redis | `Kootam.Authentication.Redis` |
| Dev/test caching | `Kootam.Caching.InMemory` |
| Production distributed cache | `Kootam.Caching.Redis` |
| Object mapping (performance) | `Kootam.AutoMap.Mapster` |
| Object mapping (convention) | `Kootam.AutoMap.AutoMapper` |
| Event-driven messaging | `Kootam.MessageBroker.RabbitMQ` |
| Static JSON translations | `Kootam.Translator.Json` |
| Database-driven i18n | `Kootam.Translator.Database` |

---

## Agent Guidelines

When adding or modifying extension usage in a backend service:

- Reference **abstractions** in Application layer; reference **implementations** only in Infrastructure or `Program.cs`.
- Pick **one implementation per abstraction** (e.g. one cache backend, one mapper).
- Copy patterns from the relevant `*.Sample` project — do not invent new DI registration shapes.
- Use the extension's `Result<T>`, `ICacheStore`, `IMessagePublisher`, etc. — do not wrap with custom abstractions unless the service already does.
- Check `nuget.config` — local packages come from `./nugets`.

---

## Related

- [Root README](../../README.md) — Core framework (Domain, Infrastructure, Presentations)
- Pack scripts: `script/win/Pack-Extensions.ps1`, `script/linux/Pack-Extensions.sh`
