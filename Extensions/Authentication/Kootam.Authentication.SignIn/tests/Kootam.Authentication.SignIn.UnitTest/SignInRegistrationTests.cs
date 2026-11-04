using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.SignIn.DependencyInjection;
using Kootam.Caching.Abstractions;
using Kootam.Email;
using Kootam.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.SignIn.UnitTest;

public class SignInRegistrationTests
{
    [Fact]
    public void AddKootamSignIn_requires_a_cache_store()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddKootamSignIn(configuration));

        Assert.Contains("AddRedisCaching", exception.Message, StringComparison.Ordinal);
        Assert.Contains("AddInMemoryCaching", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddKootamSignIn_requires_email_when_email_otp_is_enabled()
    {
        var services = RegisteredCache();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddKootamSignIn(configuration));

        Assert.Contains("AddKootamEmail", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddKootamSignIn_requires_sms_only_when_mobile_otp_is_enabled()
    {
        var services = RegisteredCache();
        services.AddSingleton<IEmailSender, RecordingEmailSender>();
        services.AddSingleton<IEmailDocumentRenderer, RecordingEmailRenderer>();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["SignIn:MobileOtp:Enabled"] = "false"
        }).Build();

        services.AddKootamSignIn(configuration);

        Assert.Contains(services, static descriptor => descriptor.ServiceType == typeof(IContactOtpService));
    }

    [Fact]
    public void AddKootamSignIn_accepts_registered_email_and_sms()
    {
        var services = RegisteredCache();
        services.AddSingleton<IEmailSender, RecordingEmailSender>();
        services.AddSingleton<IEmailDocumentRenderer, RecordingEmailRenderer>();
        services.AddSingleton<ISmsOtpSender, RecordingSmsSender>();

        services.AddKootamSignIn(new ConfigurationBuilder().Build());

        Assert.Contains(services, static descriptor => descriptor.ServiceType == typeof(IContactOtpService));
    }

    private static ServiceCollection RegisteredCache()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICacheStore, MemorySignInCache>();
        return services;
    }
}
