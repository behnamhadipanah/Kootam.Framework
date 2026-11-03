using Kootam.Authentication.SignIn.Contracts.Options;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Kootam.Authentication.SignIn.Sample;
using Kootam.Authentication.SignIn.Services;
using Kootam.Email;
using Kootam.Sms;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.SignIn.UnitTest;

public class ContactOtpServiceTests
{
    [Fact]
    public async Task Email_and_mobile_codes_are_hashed_and_verified()
    {
        var cache = new MemorySignInCache();
        var sender = new RecordingContactOtpSender();
        var service = CreateService(cache, sender, "123456");

        var email = await service.RequestAsync(ContactOtpPurpose.EmailLogin, "Ada@Example.com");
        var mobile = await service.RequestAsync(ContactOtpPurpose.MobileLogin, "09120000000");

        Assert.Equal(ContactOtpStatus.Success, email.Status);
        Assert.Equal("ada@example.com", email.Destination);
        Assert.Equal("+989120000000", mobile.Destination);
        Assert.Equal("123456", sender.LastCode);

        var verifiedEmail = await service.VerifyAsync(ContactOtpPurpose.EmailLogin, "ada@example.com", "123456");
        var verifiedMobile = await service.VerifyAsync(ContactOtpPurpose.MobileLogin, "+989120000000", "123456");
        Assert.True(verifiedEmail.Succeeded);
        Assert.True(verifiedMobile.Succeeded);

        var reused = await service.VerifyAsync(ContactOtpPurpose.EmailLogin, "ada@example.com", "123456");
        Assert.Equal(ContactOtpStatus.InvalidCode, reused.Status);
    }

    [Fact]
    public async Task Expired_code_is_rejected()
    {
        var clock = new FixedTimeProvider(new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.Zero));
        var cache = new MemorySignInCache { Clock = clock };
        var service = CreateService(cache, new RecordingContactOtpSender(), "123456", clock);
        await service.RequestAsync(ContactOtpPurpose.EmailLogin, "ada@example.com");

        clock.Move(TimeSpan.FromMinutes(11));
        cache.Clock = clock;

        var result = await service.VerifyAsync(ContactOtpPurpose.EmailLogin, "ada@example.com", "123456");
        Assert.Equal(ContactOtpStatus.InvalidCode, result.Status);
    }

    [Fact]
    public async Task Failed_delivery_removes_the_code()
    {
        var sender = new RecordingContactOtpSender { Fail = true };
        var service = CreateService(new MemorySignInCache(), sender, "123456");

        var requested = await service.RequestAsync(ContactOtpPurpose.MobileLogin, "09120000000");
        sender.Fail = false;
        var verified = await service.VerifyAsync(ContactOtpPurpose.MobileLogin, "09120000000", "123456");

        Assert.Equal(ContactOtpStatus.DeliveryFailed, requested.Status);
        Assert.Equal(ContactOtpStatus.InvalidCode, verified.Status);
    }

    private static ContactOtpService CreateService(
        MemorySignInCache cache,
        RecordingContactOtpSender sender,
        string code,
        TimeProvider? time = null)
        => new(
            cache,
            new FixedCodeGenerator(code),
            sender,
            Options.Create(new SignInOptions()),
            time ?? TimeProvider.System,
            NullLogger<ContactOtpService>.Instance);

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset utcNow = utcNow;

        public void Move(TimeSpan by) => utcNow = utcNow.Add(by);

        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}

public class SmsSignInChannelTests
{
    [Fact]
    public async Task Mobile_login_sends_the_template_code_and_not_free_text()
    {
        var sms = new RecordingSmsSender();
        var sender = new ContactOtpSender(
            Options.Create(new SignInOptions()),
            Options.Create(new EmailOptions { Mode = "Log", LogoUrl = "https://cdn.example/logo.png" }),
            Options.Create(new SmsOptions { Mode = "Log" }),
            new EmailCardRenderer(),
            new DiscardEmailSender(),
            sms,
            new SignInDevelopmentCodes());

        await sender.SendAsync(
            ContactOtpPurpose.MobileLogin,
            "+989120000000",
            "123456",
            DateTimeOffset.UtcNow.AddMinutes(10));

        Assert.Equal(1, sms.OtpCalls);
        Assert.Equal(0, sms.TextCalls);
        Assert.Equal("123456", sms.LastCode);
    }

    private sealed class RecordingSmsSender : ISmsSender
    {
        public int OtpCalls { get; private set; }

        public int TextCalls { get; private set; }

        public string? LastCode { get; private set; }

        public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
        {
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

    private sealed class DiscardEmailSender : IEmailSender
    {
        public Task SendAsync(string to, EmailDocument document, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
