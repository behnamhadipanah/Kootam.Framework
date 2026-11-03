using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Kootam.Sms.DependencyInjection;

public static class SmsServiceCollectionExtensions
{
    public static IServiceCollection AddKootamSms(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<SmsOptions>()
            .Bind(configuration.GetSection(SmsOptions.SectionName))
            .Validate(static options => options.IsLog || options.IsKavenegar, "Sms:Mode must be Log or Kavenegar.")
            .Validate(
                static options => !options.IsKavenegar || options.IsCompleteKavenegar(),
                "Sms:Mode is Kavenegar but ApiKey, Sender, OtpTemplate, BaseUrl, and TimeoutSeconds are required.")
            .ValidateOnStart();

        services.AddHttpClient(KavenegarSmsSender.HttpClientName);
        services.TryAddSingleton<ISmsSender>(static provider =>
        {
            var options = provider.GetRequiredService<IOptions<SmsOptions>>().Value;
            return options.IsKavenegar
                ? ActivatorUtilities.CreateInstance<KavenegarSmsSender>(provider)
                : ActivatorUtilities.CreateInstance<LogSmsSender>(provider);
        });
        services.TryAddSingleton<ISmsOtpSender>(static provider => provider.GetRequiredService<ISmsSender>());
        return services;
    }
}
