using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Kootam.Authentication.SignIn.Contracts.Options;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.SignIn.Services;

public sealed class ExternalSignInCatalog(IOptions<SignInOptions> options) : IExternalSignInCatalog
{
    public IReadOnlyList<ExternalProviderDescription> Describe()
    {
        var external = options.Value.External;
        var providers = new List<ExternalProviderDescription>();
        if (external.IsConfigured(ExternalSignInProvider.Google))
        {
            providers.Add(new ExternalProviderDescription(
                ExternalSignInProvider.Google.ToString(),
                external.GoogleAudiences()[0],
                Trim(external.Google.RedirectUri),
                null,
                null));
        }

        if (external.IsConfigured(ExternalSignInProvider.Apple))
        {
            providers.Add(new ExternalProviderDescription(
                ExternalSignInProvider.Apple.ToString(),
                external.Apple.EffectiveClientIds()[0],
                Trim(external.Apple.RedirectUri),
                null,
                null));
        }

        if (external.IsConfigured(ExternalSignInProvider.Microsoft))
        {
            providers.Add(new ExternalProviderDescription(
                ExternalSignInProvider.Microsoft.ToString(),
                external.Microsoft.EffectiveClientIds()[0],
                Trim(external.Microsoft.RedirectUri),
                string.IsNullOrWhiteSpace(external.Microsoft.Tenant) ? "common" : external.Microsoft.Tenant.Trim(),
                null));
        }

        if (external.IsConfigured(ExternalSignInProvider.Facebook))
        {
            providers.Add(new ExternalProviderDescription(
                ExternalSignInProvider.Facebook.ToString(),
                external.Facebook.AppId!.Trim(),
                Trim(external.Facebook.RedirectUri),
                null,
                null));
        }

        if (external.IsConfigured(ExternalSignInProvider.Telegram))
        {
            providers.Add(new ExternalProviderDescription(
                ExternalSignInProvider.Telegram.ToString(),
                null,
                Trim(external.Telegram.RedirectUri),
                null,
                BotId(external.Telegram.BotToken)));
        }

        return providers;
    }

    private static string? Trim(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? BotId(string? botToken)
    {
        var token = Trim(botToken);
        if (token is null)
            return null;

        var separator = token.IndexOf(':');
        if (separator <= 0)
            return null;

        var botId = token[..separator];
        return botId.All(char.IsAsciiDigit) ? botId : null;
    }
}
