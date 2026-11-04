using Kootam.Authentication.SignIn.Configuration;
using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.SignIn.Services;
using Kootam.Caching.Abstractions;
using Kootam.Email;
using Kootam.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kootam.Authentication.SignIn.DependencyInjection;

public static class SignInServiceCollectionExtensions
{
    public static IServiceCollection AddKootamSignIn(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        if (!services.Any(static descriptor => descriptor.ServiceType == typeof(ICacheStore)))
        {
            throw new InvalidOperationException(
                "Register a Kootam ICacheStore before AddKootamSignIn. Call AddRedisCaching when the project uses Redis, or AddInMemoryCaching otherwise.");
        }

        var configured = configuration.GetSection(SignInOptions.SectionName).Get<SignInOptions>() ?? new SignInOptions();
        if (configured.EmailOtp.Enabled
            && (!services.Any(static descriptor => descriptor.ServiceType == typeof(IEmailSender))
                || !services.Any(static descriptor => descriptor.ServiceType == typeof(IEmailDocumentRenderer))))
        {
            throw new InvalidOperationException(
                "Call AddKootamEmail before AddKootamSignIn when SignIn:EmailOtp is enabled.");
        }

        if (configured.MobileOtp.Enabled
            && !services.Any(static descriptor => descriptor.ServiceType == typeof(ISmsOtpSender)))
        {
            throw new InvalidOperationException(
                "Call AddKootamSms before AddKootamSignIn when SignIn:MobileOtp is enabled.");
        }

        services.AddOptions<SignInOptions>()
            .Bind(configuration.GetSection(SignInOptions.SectionName));
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IContactOtpCodeGenerator, NumericContactOtpCodeGenerator>();
        services.TryAddSingleton<IContactOtpService, ContactOtpService>();
        services.AddHttpClient(ExternalIdentityVerifier.HttpClientName);
        services.TryAddSingleton<IExternalIdentityVerifier, ExternalIdentityVerifier>();
        services.TryAddSingleton<IExternalSignInCatalog, ExternalSignInCatalog>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<Microsoft.Extensions.Hosting.IHostedService, ExternalSignInStartupLogger>());
        return services;
    }
}
