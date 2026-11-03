using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Kootam.Email.DependencyInjection;

public static class EmailServiceCollectionExtensions
{
    public static IServiceCollection AddKootamEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .Validate(static options => options.IsLog || options.IsSmtp, "Email:Mode must be Log or Smtp.")
            .Validate(
                static options => !options.IsSmtp || options.SmtpIsReady(),
                "Email:Mode is Smtp but Host, Port, FromAddress, User, and Password are required.")
            .ValidateOnStart();

        services.TryAddSingleton<IEmailDocumentRenderer, EmailCardRenderer>();
        services.TryAddSingleton<IEmailSender>(static provider =>
        {
            var options = provider.GetRequiredService<IOptions<EmailOptions>>().Value;
            return options.IsSmtp
                ? ActivatorUtilities.CreateInstance<SmtpEmailSender>(provider)
                : ActivatorUtilities.CreateInstance<LogEmailSender>(provider);
        });

        return services;
    }
}
