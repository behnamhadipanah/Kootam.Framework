# Kootam.Framework.Utilities

Shared utilities: API response envelope, domain exceptions, and extension methods (strings, dates, files). Primary utility package for Kootam backend services.

| Property | Value |
|----------|-------|
| **NuGet** | `Kootam.Framework.Utilities` |
| **Path** | `Common/Kootam.Framework.Utilities.Common/` |
| **Target** | `net10.0` |
| **Depends on** | `Microsoft.AspNetCore.Http` |

Used by `Kootam.Framework.Infrastructure` and `Kootam.Framework.Presentations`.

---

## ApiResponse

Standard HTTP response envelope for all API endpoints.

```csharp
using Kootam.Framework.Utilities.Responses;

// Success without data
return ApiResponse.Ok("Operation completed");

// Success with data
return ApiResponse<ProductDto>.Ok(product);

// Failure
return ApiResponse.Fail(StatusCodes.Status404NotFound, "Product not found");
return ApiResponse.Fail("Validation failed");  // defaults to 400
```

### Shape

```json
{
  "success": true,
  "statusCode": 200,
  "message": null,
  "data": { }
}
```

Integrates with CQRS via `Kootam.Framework.Presentations.Responses.ApiResponseExtensions`:

```csharp
return result.ToActionResult();      // Result → IActionResult
return result.ToActionResult<T>();   // Result<T> → IActionResult
```

---

## Exceptions

Use these in repositories and domain services for known error conditions:

| Exception | When to use |
|-----------|-------------|
| `NotExistsException` | Entity not found by id/key |
| `DuplicatedRecordException` | Unique constraint / duplicate key |
| `NationalCodeException` | Invalid Iranian national code |
| `PhoneNumberException` | Invalid phone number format |
| `ArgNullException` | Required argument is null (Persian message) |
| `ArgOutOfRangeException` | Argument out of valid range |

```csharp
if (product is null)
    throw new NotExistsException($"Product {id} not found");

if (await repo.ExistsAsync(p => p.Code == command.Code))
    throw new DuplicatedRecordException("Product code already exists");
```

Prefer returning `Result.Failure(ResultStatus.NotFound, ...)` in CQRS handlers over throwing when the failure is expected.

---

## String Extensions

Namespace: `Kootam.Framework.Utilities.Extensions`

### Persian / Arabic normalization

```csharp
var normalized = userInput.ApplyCorrectYeKe();  // Arabic ی/ك → Persian ی/ک + Fa2En
var english = "۱۲۳".Fa2En();                     // Persian digits → ASCII
var persian = "123".En2Fa();                     // ASCII → Persian digits
```

### Safe parsing

```csharp
var id = "abc".ToSafeLong(defaultValue: 0);
var count = "10".ToSafeInt();
var nullable = "x".ToSafeNullableLong();  // null if parse fails
```

### Other

| Method | Purpose |
|--------|---------|
| `ToStringOrEmpty()` | Null-safe string |
| `ToUnderscoreCase()` | `MyProperty` → `my_property` |
| `ToByteArray()` / `FromByteArray()` | UTF-8 encoding |
| `ToNumeric()` / `ToCurrency()` | Formatted numbers |

---

## Date Extensions

Namespace: `Kootam.Framework.Utilities.Extensions`

Persian (Jalali) calendar support:

```csharp
var persianDate = DateTime.Now.ConvertToPersianDate("-");           // 1403-05-16
var withTime = DateTime.Now.ConvertToPersianDate("-", hasClock: true);

var gregorian = "1403-05-16".ConvertToGregorianDate();
var gregorianWithTime = "1403/05/16 14:30:00".ConvertToGregorianDate(hasClock: true);
```

---

## Validate Extensions

```csharp
if (value.IsNullOrEmpty()) { ... }
var safe = value.IsNullOrEmpty("default");
var num = value.ToLong();  // returns long.MinValue if empty
```

---

## File Extensions

Namespace: `Kootam.Framework.Utilities.Extensions.Files`

```csharp
var fileInfo = new FileUpload
{
    DirectoryName = "products",
    Path = "uploads",
    UniqueName = Guid.NewGuid().ToString()
};

var savedPath = formFile.Upload(fileInfo);
var deleted = savedPath.Delete();
```

Files are saved under `{CurrentDirectory}/{Path}/{DirectoryName}_{UniqueName}{extension}`.

---

## Utilities vs Utilities.Tools

| | `Kootam.Framework.Utilities` | `Kootam.Framework.Utilities.Tools` |
|--|-------------------------------|-------------------------------------|
| Status | **Active — use this** | Legacy duplicate |
| Referenced by framework | Yes (Infrastructure, Presentations) | No |
| String extensions namespace | `Kootam.Framework.Utilities.Extensions` | `Kootam.Framework.Utilities.Tools.Extensions` |
| File extensions | Clean, no extra deps | Broken refs (`MediatR`, `Framework.Models`) |
| ASP.NET dependency | Yes (`IFormFile`) | No |

**Always reference `Kootam.Framework.Utilities` in new projects.** Do not add `Kootam.Framework.Utilities.Tools`.

---

## Agent Rules

- All API controllers must return `ApiResponse` / `ApiResponse<T>` or use `ToActionResult()` from Presentations.
- Apply `ApplyCorrectYeKe()` on user-entered Persian text before persistence (also handled at EF level by `SetPersianYeKeInterceptor` in Infrastructure).
- Use `Fa2En()` before parsing user-entered numeric strings.
- Throw framework exceptions in Infrastructure/repositories; map to `Result` in CQRS handlers.
- Do not duplicate exception classes or response types in consuming services.
