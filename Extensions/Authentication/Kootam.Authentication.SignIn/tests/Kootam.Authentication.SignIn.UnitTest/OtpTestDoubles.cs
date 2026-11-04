using Kootam.Authentication.Abstractions.Services;
using Kootam.Caching.Abstractions;
using Kootam.Email;
using Kootam.Sms;

namespace Kootam.Authentication.SignIn.UnitTest;

internal sealed class MemorySignInCache : ICacheStore
{
    private readonly Dictionary<string, (object Value, DateTimeOffset? Expires)> items = [];

    public TimeProvider Clock { get; set; } = TimeProvider.System;

    public Task<bool> ExistsAsync(string key) => Task.FromResult(Read(key) is not null);

    public Task<T?> GetAsync<T>(string key) where T : class => Task.FromResult(Read(key) as T);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        DateTimeOffset? expires = expiry is null ? null : Clock.GetUtcNow().Add(expiry.Value);
        items[key] = (value, expires);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key) => Task.FromResult(items.Remove(key));

    private object? Read(string key)
    {
        if (!items.TryGetValue(key, out var entry) || (entry.Expires is { } expires && expires <= Clock.GetUtcNow()))
        {
            items.Remove(key);
            return null;
        }

        return entry.Value;
    }
}

internal sealed class FixedCodeGenerator(string code) : IContactOtpCodeGenerator
{
    public string Generate() => code;
}

internal sealed class RecordingEmailRenderer : IEmailDocumentRenderer
{
    public EmailCard? LastCard { get; private set; }

    public EmailDocument Render(EmailCard card)
    {
        LastCard = card;
        return new EmailDocument(card.Subject, card.Highlight ?? "", card.Highlight ?? "");
    }
}

internal sealed class RecordingEmailSender : IEmailSender
{
    public EmailDocument? LastDocument { get; private set; }

    public bool Fail { get; set; }

    public Task SendAsync(string to, EmailDocument document, CancellationToken cancellationToken = default)
    {
        if (Fail)
            throw new InvalidOperationException("delivery failed " + document.PlainText);

        LastDocument = document;
        return Task.CompletedTask;
    }
}

internal sealed class RecordingSmsSender : ISmsSender
{
    public int OtpCalls { get; private set; }

    public int TextCalls { get; private set; }

    public string? LastCode { get; private set; }

    public bool Fail { get; set; }

    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        if (Fail)
            throw new InvalidOperationException("delivery failed " + code);

        OtpCalls++;
        LastCode = code;
        return Task.CompletedTask;
    }

    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        TextCalls++;
        return Task.CompletedTask;
    }
}
