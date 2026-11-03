using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.UnitTest;

internal sealed class MemorySignInCache : ISignInCache
{
    private readonly Dictionary<string, (object Value, DateTimeOffset? Expires)> items = [];

    public TimeProvider Clock { get; set; } = TimeProvider.System;

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        if (!items.TryGetValue(key, out var entry) || (entry.Expires is { } expires && expires <= Clock.GetUtcNow()))
        {
            items.Remove(key);
            return Task.FromResult<T?>(null);
        }

        return Task.FromResult(entry.Value as T);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        DateTimeOffset? expires = expiry is null ? null : Clock.GetUtcNow().Add(expiry.Value);
        items[key] = (value, expires);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        items.Remove(key);
        return Task.CompletedTask;
    }
}

internal sealed class FixedCodeGenerator(string code) : IContactOtpCodeGenerator
{
    public string Generate() => code;
}

internal sealed class RecordingContactOtpSender : IContactOtpSender
{
    public string? LastCode { get; private set; }

    public ContactOtpPurpose? LastPurpose { get; private set; }

    public bool Fail { get; set; }

    public Task SendAsync(
        ContactOtpPurpose purpose,
        string destination,
        string code,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (Fail)
            throw new InvalidOperationException("delivery failed " + code);

        LastPurpose = purpose;
        LastCode = code;
        return Task.CompletedTask;
    }
}
