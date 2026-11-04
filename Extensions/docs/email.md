# ایمیل

`Kootam.Email` قالب نامه و ارسال آن را جدا از ورود می‌سازد. ورودی قالب لوگو، subject، متن، عنوان، مقدار برجسته (کد یا لینک) و پاصفحه است.

## نصب

```xml
<ProjectReference Include="path/to/Kootam.Email.csproj" />
```

یا بعد از pack محلی:

```xml
<PackageReference Include="Kootam.Email" Version="1.0.0" />
```

## ثبت

```csharp
builder.Services.AddKootamEmail(builder.Configuration);
```

## تنظیمات

`Email:Mode` فقط `Log` یا `Smtp` است. `Log` نامه را تحویل نمی‌دهد و برای توسعه است؛ متن سادهٔ نامه، از جمله کد ورود، در لاگ می‌آید. `Smtp` بدون Host، Port، FromAddress، User و Password در startup خطا می‌دهد.

```json
{
  "Email": {
    "Mode": "Smtp",
    "LogoUrl": "https://cdn.example/logo.png",
    "FromName": "Example",
    "FromAddress": "auth@example.com",
    "Smtp": {
      "Host": "mail.example.com",
      "ConnectHost": "",
      "Port": 587,
      "ImplicitSslPort": 465,
      "UseSsl": true,
      "User": "",
      "Password": "",
      "TimeoutSeconds": 15
    }
  }
}
```

`ConnectHost` اختیاری است. وقتی نام SMTP رکورد عمومی ندارد، اتصال TCP به این آدرس می‌رود و نام گواهی همان `Host` می‌ماند. رمز را در مخزن نگذارید.

`Port` پیش‌فرض ۵۸۷ است و هر پورت ۱ تا ۶۵۵۳۵ را می‌پذیرد. TLS ضمنی (اتصال از همان ابتدا رمزنگاری‌شده) پیش‌فرض فقط وقتی است که `Port` برابر `ImplicitSslPort` باشد؛ مقدار پیش‌فرض `ImplicitSslPort` برابر ۴۶۵ است. اگر سرور TLS ضمنی را روی پورت دیگری گوش می‌دهد، هر دو را همان پورت بگذارید. `ImplicitSslPort` با مقدار ۰ این حالت را خاموش می‌کند و `UseSsl` فقط STARTTLS یا اتصال ساده را انتخاب می‌کند.

## ساخت نامه

```csharp
public sealed class WelcomeMail(IEmailDocumentRenderer renderer, IEmailSender sender)
{
    public async Task SendAsync(string to, CancellationToken cancellationToken)
    {
        var document = renderer.Render(new EmailCard(
            Subject: "Welcome",
            LogoUrl: "https://cdn.example/logo.png",
            Body: "Your account is ready.",
            Title: "Welcome",
            Highlight: "Open the app",
            HighlightHref: "https://app.example/login",
            Footer: "If you did not ask for this message, ignore it."));

        await sender.SendAsync(to, document, cancellationToken);
    }
}
```

لوگو فقط با آدرس `http` یا `https` در نامه می‌آید. متن‌ها encode می‌شوند. `Highlight` بدون لینک درشت نشان داده می‌شود و برای کد ورود مناسب است. با `HighlightHref` به لینک تبدیل می‌شود.

`EmailDocument` سه بخش دارد: `Subject`، `PlainText` و `Html`. فرستنده هر دو متن را با هم می‌فرستد.
