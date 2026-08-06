# Translator Extension

Localization/i18n with a unified `ITranslator` interface. Two storage backends: JSON files or SQL database.

| Package | Storage | Sample |
|---------|---------|--------|
| `Kootam.Translator.Abstractions` | Contracts | — |
| `Kootam.Translator.Json` | JSON translation files | `Extensions/Translator/Kootam.Translator.Json/src/Kootam.Translator.Json.Sample/` |
| `Kootam.Translator.Database` | SQL Server (Dapper) | `Extensions/Translator/Kootam.Translator.Database/src/Kootam.Translator.Database.Sample/` |

---

## Abstraction

```csharp
public interface ITranslator
{
    string this[string key] { get; }
    string this[string key, params object[] arguments] { get; }

    string Get(string key);
    string Get(string key, params object[] arguments);
    string Get(string key, CultureInfo culture);
    string Get(string key, CultureInfo culture, params object[] arguments);
}
```

Supporting interface: `ITranslationStore` — low-level key/value retrieval.

---

## JSON Backend

Best for static translations shipped with the application.

**Structure:** JSON files per culture (e.g. `Resources/en.json`, `Resources/fa.json`).

Register via `AddTranslator` in `Kootam.Translator.Json.DependencyInjection`:

```csharp
using Kootam.Translator.Json.DependencyInjection;

builder.Services.AddTranslator(builder.Configuration);
// or
builder.Services.AddTranslator(options =>
{
    options.ResourcesPath = "Resources";
    options.DefaultCulture = "en";
});
```

Configure `JsonTranslatorOptions` section in `appsettings.json`.

---

## Database Backend

Best for dynamic translations managed via admin UI.

Register via `Kootam.Translator.Database.DependencyInjection`:

```csharp
using Kootam.Translator.Database.DependencyInjection;

builder.Services.AddTranslator(builder.Configuration);
// or
builder.Services.AddTranslator(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("Default");
});
```

Uses Dapper against a localization table. Model: `LocalizationRecord` (key, culture, value).

**Options class:** `TranslatorOptions` with `ConnectionString` and `DefaultTranslatorOptionsName`.

---

## Usage

Inject `ITranslator` in controllers, handlers, or services:

```csharp
public class WelcomeController(ITranslator translator) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var message = translator.Get("WelcomeMessage");
        var formatted = translator.Get("HelloUser", userName);

        // Indexer syntax
        var alt = translator["WelcomeMessage"];

        return Ok(new { message, formatted });
    }
}
```

Culture-specific:

```csharp
var faMessage = translator.Get("WelcomeMessage", new CultureInfo("fa-IR"));
```

---

## Choosing a Backend

| Criteria | JSON | Database |
|----------|------|----------|
| Static app strings | Yes | Overkill |
| Admin-editable text | No | Yes |
| Deployment | Files in repo | DB migration |
| Performance | Fast (in-memory cache) | DB round-trip |

Pick **one** backend per service.

---

## Agent Rules

- Use `ITranslator` — do not hardcode user-facing strings in controllers/handlers.
- Translation keys use PascalCase or dot notation: `Validation.Required`, `Product.NotFound`.
- Parameterized strings: `translator.Get("HelloUser", userName)` — use `{0}` placeholders in resource values.
- JSON backend for new services unless dynamic translation is a requirement.
- Do not reference Dapper or file paths in Application layer — only inject `ITranslator`.
