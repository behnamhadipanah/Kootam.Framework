# ورود با کد و SSO

`Kootam.Authentication.SignIn` سه چیز را تأیید می‌کند و کاربر نمی‌سازد:

- کد ایمیل
- کد پیامکی
- هویت Google، Apple، Microsoft، Facebook و Telegram

بعد از تأیید، اپ کاربر را پیدا یا می‌سازد و با `Kootam.Authentication` و `Kootam.Authentication.Jwt` توکن می‌دهد.

## نصب و ثبت

کش Kootam باید قبل از `AddKootamSignIn` ثبت شده باشد. ورود همان `ICacheStore` را مصرف می‌کند. اگر Redis در پروژه است و لازم است، `AddRedisCaching` را صدا بزنید. در غیر این صورت `AddInMemoryCaching`.

ارسال از پکیج‌های موجود Email و Sms است. ورود نامه را با `IEmailDocumentRenderer` و `IEmailSender` می‌سازد و کد موبایل را با `ISmsOtpSender` می‌فرستد. اگر `EmailOtp` روشن باشد و ایمیل ثبت نشده باشد، یا `MobileOtp` روشن باشد و پیامک ثبت نشده باشد، `AddKootamSignIn` خطا می‌دهد. کانال دیگری با جایگزینی همان `IEmailSender` یا `ISmsOtpSender` وصل می‌شود.

```csharp
builder.Services.AddInMemoryCaching(); // یا AddRedisCaching(builder.Configuration)
builder.Services.AddKootamEmail(builder.Configuration); // فقط وقتی ورود با ایمیل لازم است
builder.Services.AddKootamSms(builder.Configuration);   // فقط وقتی ورود با پیامک لازم است
builder.Services.AddKootamSignIn(builder.Configuration);
```

## تنظیمات

```json
{
  "Email": {
    "Mode": "Log",
    "LogoUrl": "https://cdn.example/logo.png",
    "FromAddress": "auth@example.com"
  },
  "SignIn": {
    "MaxRequestsPerWindow": 5,
    "RequestWindowMinutes": 10,
    "MaxVerificationAttempts": 5,
    "EmailOtp": {
      "Enabled": true,
      "CodeExpirationMinutes": 10,
      "Subject": "Sign-in code",
      "Body": "Use this code to continue.",
      "Footer": "If you did not request this code, you can ignore this message."
    },
    "MobileOtp": {
      "Enabled": true,
      "CodeExpirationMinutes": 10
    },
    "External": {
      "Google": { "Enabled": true, "ClientIds": ["web.apps.googleusercontent.com"] },
      "Apple": { "Enabled": false, "ClientId": "" },
      "Microsoft": { "Enabled": false, "ClientId": "", "Tenant": "common" },
      "Facebook": { "Enabled": false, "AppId": "", "AppSecret": "" },
      "Telegram": { "Enabled": false, "BotToken": "" }
    }
  },
  "Sms": { "Mode": "Log" }
}
```

تنظیم نامه و پیامک مال همین پکیج نیست. `Email` و `Sms` را خودِ آن پکیج‌ها اعتبارسنجی می‌کنند. provider خارجیِ روشن ولی بدون credential فقط هشدار لاگ می‌شود و میزبان را متوقف نمی‌کند. شناسهٔ Google باید به `.apps.googleusercontent.com` ختم شود. مقدار `GOCSPX-` راز کلاینت است، نه شناسه.

در production به‌جای `Log` از `Smtp` و `Kavenegar` استفاده کنید. جزئیات در [email.md](email.md) و [sms.md](sms.md) است.

## ورود با کد ایمیل

```csharp
await otp.RequestAsync(ContactOtpPurpose.EmailLogin, email);
var verified = await otp.VerifyAsync(ContactOtpPurpose.EmailLogin, email, code);
if (!verified.Succeeded)
    return Results.BadRequest(verified.Status);

// verified.Destination is the normalized email.
// Find or create the user, then issue a Kootam JWT.
```

کد هش می‌شود. پاسخ درخواست، خود کد را برنمی‌گرداند. نامه لوگوی `Email:LogoUrl`، subject و متن `EmailOtp` را دارد و کد در بخش برجسته است.

## ورود با کد پیامکی

```csharp
await otp.RequestAsync(ContactOtpPurpose.MobileLogin, phoneNumber);
var verified = await otp.VerifyAsync(ContactOtpPurpose.MobileLogin, phoneNumber, code);
```

1. شماره نرمال می‌شود. `0912...` و `+98912...` یکی هستند.
2. درخواست اضافه در پنجرهٔ زمانی `RateLimited` است.
3. هش کد تا زمان انقضا در کش می‌ماند.
4. همان کد با `ISmsOtpSender.SendOtpAsync` و قالب کاوه‌نگار می‌رود و متن آزاد فرستاده نمی‌شود.
5. اگر ارسال شکست بخورد، کد پاک می‌شود و نتیجه `DeliveryFailed` است.
6. `VerifyAsync` فقط شمارهٔ تأییدشده را برمی‌گرداند. کد یک‌بارمصرف است.

`InvalidCode` هم برای کد غلط است و هم برای کد منقضی. بعد از `MaxVerificationAttempts` نتیجه `RateLimited` است.

## حالت Log در توسعه

`Email:Mode` و `Sms:Mode` برابر `Log` کد را تحویل نمی‌دهند. پیامک کد را در لاگ می‌نویسد. نامه متن ساده را، که کد داخل آن است، در لاگ می‌نویسد. پاسخ API کد را برنمی‌گرداند. در production این حالت را خاموش کنید.

## SSO

کلاینت توکن یا ویجت را می‌گیرد و به API می‌دهد. این پکیج مسیر redirect سرور ندارد.

```csharp
var identity = await verifier.VerifyAsync(new ExternalSignInRequest(
    ExternalSignInProvider.Google,
    IdToken: idToken));

if (!identity.Succeeded)
    return Results.Unauthorized();

// identity.Identity.Subject is the provider user id.
```

| Provider | ورودی | بررسی |
| --- | --- | --- |
| Google، Apple، Microsoft | `IdToken` | امضای JWKS، مخاطب، صادرکننده و عمر توکن |
| Facebook | `AccessToken` | `debug_token` و سپس پروفایل |
| Telegram | `Telegram` | HMAC ویجت، حداکثر ۲۴ ساعت |

`IExternalSignInCatalog.Describe()` شناسهٔ عمومی کلاینت‌ها را برای دکمهٔ ورود برمی‌گرداند و رازها را برنمی‌گرداند.

نتیجهٔ `Disabled` یعنی provider در تنظیمات کامل نیست. `Failed` یعنی توکن یا امضا پذیرفته نشد.

## نمونه

پروژهٔ `Kootam.Authentication.SignIn.Sample` هر دو ورود را در حالت `Log` نشان می‌دهد:

- `POST /sign-in/email/request` و `POST /sign-in/email/verify`
- `POST /sign-in/mobile/request` و `POST /sign-in/mobile/verify`
- `GET /email/preview`

بدنهٔ درخواست: `{ "destination": "..." }`. تأیید: `{ "destination": "...", "code": "..." }`.
