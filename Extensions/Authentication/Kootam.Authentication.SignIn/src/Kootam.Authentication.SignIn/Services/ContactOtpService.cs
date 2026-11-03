using System.Security.Cryptography;
using System.Text;
using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Kootam.Authentication.SignIn.Contracts.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.SignIn.Services;

public sealed class ContactOtpService(
    ISignInCache cache,
    IContactOtpCodeGenerator codeGenerator,
    IContactOtpSender sender,
    IOptions<SignInOptions> options,
    TimeProvider timeProvider,
    ILogger<ContactOtpService> logger) : IContactOtpService
{
    public async Task<ContactOtpResult> RequestAsync(
        ContactOtpPurpose purpose,
        string destination,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var settings = options.Value;
        if (!settings.IsEnabled(purpose))
            return ContactOtpResult.Fail(ContactOtpStatus.Disabled);

        if (!ContactAddress.TryNormalize(purpose, destination, out var normalized))
            return ContactOtpResult.Fail(ContactOtpStatus.InvalidDestination);

        var now = timeProvider.GetUtcNow();
        var maxRequests = Math.Max(1, settings.MaxRequestsPerWindow);
        var window = TimeSpan.FromMinutes(Math.Max(1, settings.RequestWindowMinutes));
        var rateKey = RateKey(purpose, normalized);
        var rate = await cache.GetAsync<ContactOtpRateEntry>(rateKey);
        var count = rate is null || now - rate.WindowStartedAt >= window ? 0 : rate.Count;
        if (count >= maxRequests)
            return ContactOtpResult.Fail(ContactOtpStatus.RateLimited);

        var windowStartedAt = count == 0 ? now : rate!.WindowStartedAt;
        await cache.SetAsync(rateKey, new ContactOtpRateEntry(count + 1, windowStartedAt), window);

        var plainCode = codeGenerator.Generate();
        var lifetime = TimeSpan.FromMinutes(Math.Max(1, settings.Method(purpose).CodeExpirationMinutes));
        var expiresAt = now.Add(lifetime);
        var codeKey = CodeKey(purpose, normalized);
        await cache.SetAsync(
            codeKey,
            new ContactOtpCacheEntry(purpose, normalized, OtpCodeHasher.Hash(plainCode), expiresAt, 0),
            lifetime);

        try
        {
            await sender.SendAsync(purpose, normalized, plainCode, expiresAt, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await cache.RemoveAsync(codeKey);
            logger.LogWarning(
                "Contact OTP delivery failed for {Purpose}. ExceptionType={ExceptionType} Detail={Detail}",
                purpose,
                exception.GetType().Name,
                SafeDetail(exception.Message, plainCode));
            return ContactOtpResult.Fail(ContactOtpStatus.DeliveryFailed);
        }

        return ContactOtpResult.Success(normalized);
    }

    public async Task<ContactOtpResult> VerifyAsync(
        ContactOtpPurpose purpose,
        string destination,
        string code,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!options.Value.IsEnabled(purpose))
            return ContactOtpResult.Fail(ContactOtpStatus.Disabled);

        if (!ContactAddress.TryNormalize(purpose, destination, out var normalized) || string.IsNullOrWhiteSpace(code))
            return ContactOtpResult.Fail(ContactOtpStatus.InvalidCode);

        var codeKey = CodeKey(purpose, normalized);
        var entry = await cache.GetAsync<ContactOtpCacheEntry>(codeKey);
        if (entry is null
            || entry.Purpose != purpose
            || !string.Equals(entry.Destination, normalized, StringComparison.Ordinal))
            return ContactOtpResult.Fail(ContactOtpStatus.InvalidCode);

        var now = timeProvider.GetUtcNow();
        if (entry.ExpiresAt <= now)
        {
            await cache.RemoveAsync(codeKey);
            return ContactOtpResult.Fail(ContactOtpStatus.InvalidCode);
        }

        var maxAttempts = Math.Max(1, options.Value.MaxVerificationAttempts);
        if (entry.FailedAttempts >= maxAttempts)
        {
            await cache.RemoveAsync(codeKey);
            return ContactOtpResult.Fail(ContactOtpStatus.RateLimited);
        }

        if (!OtpCodeHasher.Verify(code.Trim(), entry.CodeHash))
        {
            var remaining = entry.ExpiresAt - now;
            if (remaining < TimeSpan.FromSeconds(1))
                remaining = TimeSpan.FromSeconds(1);

            await cache.SetAsync(
                codeKey,
                entry with { FailedAttempts = entry.FailedAttempts + 1 },
                remaining);
            return ContactOtpResult.Fail(ContactOtpStatus.InvalidCode);
        }

        await cache.RemoveAsync(codeKey);
        return ContactOtpResult.Success(normalized);
    }

    private static string SafeDetail(string? message, string plainCode)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "unavailable";

        return string.IsNullOrEmpty(plainCode)
            ? message
            : message.Replace(plainCode, "[redacted]", StringComparison.Ordinal);
    }

    private static string CodeKey(ContactOtpPurpose purpose, string destination)
        => $"auth:contact-otp:{Fingerprint(purpose, destination)}";

    private static string RateKey(ContactOtpPurpose purpose, string destination)
        => $"auth:contact-otp-rate:{Fingerprint(purpose, destination)}";

    private static string Fingerprint(ContactOtpPurpose purpose, string destination)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{purpose}\n{destination}"));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
