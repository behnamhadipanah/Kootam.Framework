using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.SignIn.Services;
using Kootam.Email;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.SignIn.UnitTest;

public class ContactOtpServiceTests
{
    [Fact]
    public async Task Email_and_mobile_codes_are_hashed_and_verified()
    {
        var cache = new MemorySignInCache();
        var email = new RecordingEmailSender();
        var sms = new RecordingSmsSender();
        var service = CreateService(cache, email, sms, "123456");

        var emailResult = await service.RequestAsync(ContactOtpPurpose.EmailLogin, "Ada@Example.com");
        var mobileResult = await service.RequestAsync(ContactOtpPurpose.MobileLogin, "09120000000");

        Assert.Equal(ContactOtpStatus.Success, emailResult.Status);
        Assert.Equal("ada@example.com", emailResult.Destination);
        Assert.Equal("+989120000000", mobileResult.Destination);
        Assert.Equal("123456", email.LastDocument?.PlainText);
        Assert.Equal("123456", sms.LastCode);
        Assert.Equal(1, sms.OtpCalls);
        Assert.Equal(0, sms.TextCalls);

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
        var service = CreateService(cache, new RecordingEmailSender(), new RecordingSmsSender(), "123456", clock);
        await service.RequestAsync(ContactOtpPurpose.EmailLogin, "ada@example.com");

        clock.Move(TimeSpan.FromMinutes(11));
        cache.Clock = clock;

        var result = await service.VerifyAsync(ContactOtpPurpose.EmailLogin, "ada@example.com", "123456");
        Assert.Equal(ContactOtpStatus.InvalidCode, result.Status);
    }

    [Fact]
    public async Task Failed_delivery_removes_the_code()
    {
        var sms = new RecordingSmsSender { Fail = true };
        var service = CreateService(new MemorySignInCache(), new RecordingEmailSender(), sms, "123456");

        var requested = await service.RequestAsync(ContactOtpPurpose.MobileLogin, "09120000000");
        sms.Fail = false;
        var verified = await service.VerifyAsync(ContactOtpPurpose.MobileLogin, "09120000000", "123456");

        Assert.Equal(ContactOtpStatus.DeliveryFailed, requested.Status);
        Assert.Equal(ContactOtpStatus.InvalidCode, verified.Status);
    }

    [Fact]
    public async Task Email_uses_the_sign_in_copy_and_logo()
    {
        var renderer = new RecordingEmailRenderer();
        var service = CreateService(
            new MemorySignInCache(),
            new RecordingEmailSender(),
            new RecordingSmsSender(),
            "123456",
            renderer: renderer,
            emailOptions: new EmailOptions { LogoUrl = "https://cdn.example/logo.png" });

        await service.RequestAsync(ContactOtpPurpose.EmailLogin, "ada@example.com");

        Assert.NotNull(renderer.LastCard);
        Assert.Equal("Sign-in code", renderer.LastCard.Subject);
        Assert.Equal("https://cdn.example/logo.png", renderer.LastCard.LogoUrl);
        Assert.Equal("123456", renderer.LastCard.Highlight);
    }

    private static ContactOtpService CreateService(
        MemorySignInCache cache,
        RecordingEmailSender email,
        RecordingSmsSender sms,
        string code,
        TimeProvider? time = null,
        RecordingEmailRenderer? renderer = null,
        EmailOptions? emailOptions = null)
        => new(
            cache,
            new FixedCodeGenerator(code),
            Options.Create(new SignInOptions()),
            Options.Create(emailOptions ?? new EmailOptions()),
            time ?? TimeProvider.System,
            NullLogger<ContactOtpService>.Instance,
            renderer ?? new RecordingEmailRenderer(),
            email,
            sms);

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset utcNow = utcNow;

        public void Move(TimeSpan by) => utcNow = utcNow.Add(by);

        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
