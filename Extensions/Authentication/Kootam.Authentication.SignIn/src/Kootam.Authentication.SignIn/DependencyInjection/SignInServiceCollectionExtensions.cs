using Kootam.Authentication.SignIn.Configuration;
using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.Contracts.Options;
using Kootam.Authentication.SignIn.Models;
using Kootam.Authentication.SignIn.Services;
using Kootam.Caching.Abstractions;
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

        var configured = configuration.GetSection(SignInOptions.SectionName).Get<SignInOptions>() ?? new SignInOptions();
        if ((configured.EmailOtp.Enabled || configured.MobileOtp.Enabled)
            && !services.Any(descriptor => descriptor.ServiceType == typeof(IContactOtpSender)))
        {
            throw new InvalidOperationException(
                "Register IContactOtpSender before AddKootamSignIn when SignIn:EmailOtp or SignIn:MobileOtp is enabled.");
        }

        services.AddOptions<SignInOptions>()
            .Bind(configuration.GetSection(SignInOptions.SectionName));

        services.TryAddSingleton<ISignInCache>(static provider =>
        {
            if (provider.GetService<ICacheStore>() is { } store)
                return new SignInCache(store);

            if (provider.GetService<ICacheAdapter>() is ICacheStore adapter)
                return new SignInCache(adapter);

            throw new InvalidOperationException(
                "Register a Kootam ICacheStore or ICacheAdapter before using sign-in codes.");
        });
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<SignInDevelopmentCodes>();
        services.TryAddSingleton<IContactOtpCodeGenerator, NumericContactOtpCodeGenerator>();
        services.TryAddSingleton<IContactOtpService, ContactOtpService>();
        services.AddHttpClient(ExternalIdentityVerifier.HttpClientName);
        services.TryAddSingleton<IExternalIdentityVerifier, ExternalIdentityVerifier>();
        services.TryAddSingleton<IExternalSignInCatalog, ExternalSignInCatalog>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<Microsoft.Extensions.Hosting.IHostedService, ExternalSignInStartupLogger>());
        return services;
    }
}
