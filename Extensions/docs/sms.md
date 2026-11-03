# پیامک

`Kootam.Sms` ارسال پیامک را جدا از ورود می‌سازد و به هر پروژه‌ای که لازم دارد اضافه می‌شود. دو کار جدا دارد:

- `ISmsOtpSender.SendOtpAsync` یک کد را با قالب کاوه‌نگار (`VerifyLookup`) می‌فرستد.
- `ISmsSender.SendAsync` متن آزاد را می‌فرستد و قالب کد را صدا نمی‌زند.

## نصب

```xml
<PackageReference Include="Kootam.Sms" Version="1.0.0" />
```

## ثبت

```csharp
builder.Services.AddKootamSms(builder.Configuration);
```

بخش تنظیمات `Sms` است.

## حالت Log

```json
{
  "Sms": { "Mode": "Log" }
}
```

کد در لاگ می‌آید و به کاوه‌نگار نمی‌رود. این حالت را در production روشن نکنید.

## کاوه‌نگار

`Mode` برابر `Kavenegar` بدون `ApiKey`، `Sender`، `OtpTemplate`، `BaseUrl` و `TimeoutSeconds` در startup خطا می‌دهد.

```json
{
  "Sms": {
    "Mode": "Kavenegar",
    "Kavenegar": {
      "ApiKey": "",
      "Sender": "1000",
      "OtpTemplate": "verify-code",
      "BaseUrl": "https://api.kavenegar.com/v1",
      "TimeoutSeconds": 15
    }
  }
}
```

`OtpTemplate` نام الگوی تأییدشده در پنل کاوه‌نگار است. کد فقط داخل همان الگو و با `VerifyLookup` می‌رود. توکن الگو باید کد را نشان بدهد.

شماره پیش از ارسال نرمال می‌شود. `0912...` به `+98912...` تبدیل می‌شود. شمارهٔ نامعتبر `SmsDeliveryException` با نوع `InvalidParameters` می‌دهد.

## متن آزاد

```csharp
await sms.SendAsync("+989120000000", "Your profile was updated.", cancellationToken);
```

این مسیر قالب کد را صدا نمی‌زند.
