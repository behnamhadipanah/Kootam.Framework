using System.Collections.Concurrent;
using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.DependencyInjection;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Kootam.Authentication.SignIn.Sample;
using Kootam.Caching.Abstractions;
using Kootam.Email;
using Kootam.Email.DependencyInjection;
using Kootam.Sms.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ICacheStore, ExpiringMemoryCache>();
builder.Services.AddKootamEmail(builder.Configuration);
builder.Services.AddKootamSms(builder.Configuration);
builder.Services.AddSingleton<IContactOtpSender, ContactOtpSender>();
builder.Services.AddKootamSignIn(builder.Configuration);

var app = builder.Build();

app.MapPost("/sign-in/email/request", async (DestinationBody body, IContactOtpService otp, SignInDevelopmentCodes codes) =>
{
    var result = await otp.RequestAsync(ContactOtpPurpose.EmailLogin, body.Destination);
    return Results.Ok(new { result.Status, result.Destination, developmentCode = codes.LastEmailCode });
});

app.MapPost("/sign-in/email/verify", async (VerifyBody body, IContactOtpService otp) =>
    Results.Ok(await otp.VerifyAsync(ContactOtpPurpose.EmailLogin, body.Destination, body.Code)));

app.MapPost("/sign-in/mobile/request", async (DestinationBody body, IContactOtpService otp, SignInDevelopmentCodes codes) =>
{
    var result = await otp.RequestAsync(ContactOtpPurpose.MobileLogin, body.Destination);
    return Results.Ok(new { result.Status, result.Destination, developmentCode = codes.LastSmsCode });
});

app.MapPost("/sign-in/mobile/verify", async (VerifyBody body, IContactOtpService otp) =>
    Results.Ok(await otp.VerifyAsync(ContactOtpPurpose.MobileLogin, body.Destination, body.Code)));

app.MapGet("/email/preview", (IEmailDocumentRenderer renderer, IOptions<EmailOptions> email) =>
{
    var document = renderer.Render(new EmailCard(
        Subject: "Sign-in code",
        LogoUrl: email.Value.LogoUrl,
        Body: "Use this code to continue.",
        Highlight: "123456",
        Footer: "This code expires in 10 minutes."));
    return Results.Content(document.Html, "text/html");
});

app.Run();

internal sealed record DestinationBody(string Destination);

internal sealed record VerifyBody(string Destination, string Code);

internal sealed class ExpiringMemoryCache : ICacheStore
{
    private readonly ConcurrentDictionary<string, (object Value, DateTimeOffset? Expires)> items = new();

    public Task<bool> ExistsAsync(string key) => Task.FromResult(Get(key) is not null);

    public Task<T?> GetAsync<T>(string key) where T : class => Task.FromResult(Get(key) as T);

    public Task<bool> RemoveAsync(string key) => Task.FromResult(items.TryRemove(key, out _));

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        DateTimeOffset? expires = expiry is null ? null : DateTimeOffset.UtcNow.Add(expiry.Value);
        items[key] = (value, expires);
        return Task.CompletedTask;
    }

    private object? Get(string key)
    {
        if (!items.TryGetValue(key, out var entry))
            return null;

        if (entry.Expires is { } expires && expires <= DateTimeOffset.UtcNow)
        {
            items.TryRemove(key, out _);
            return null;
        }

        return entry.Value;
    }
}
