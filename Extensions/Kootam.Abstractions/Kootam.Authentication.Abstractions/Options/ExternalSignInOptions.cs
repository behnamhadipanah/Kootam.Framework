using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.Contracts.Options;

public sealed class ExternalSignInOptions
{
    public const string GoogleWebClientSuffix = ".apps.googleusercontent.com";

    public OidcSignInOptions Google { get; set; } = new();

    public OidcSignInOptions Apple { get; set; } = new();

    public MicrosoftSignInOptions Microsoft { get; set; } = new();

    public FacebookSignInOptions Facebook { get; set; } = new();

    public TelegramSignInOptions Telegram { get; set; } = new();

    public bool IsConfigured(ExternalSignInProvider provider)
        => provider switch
        {
            ExternalSignInProvider.Google => GoogleAudiences().Count > 0,
            ExternalSignInProvider.Apple => Apple.IsConfigured(),
            ExternalSignInProvider.Microsoft => Microsoft.IsConfigured(),
            ExternalSignInProvider.Facebook => Facebook.IsConfigured(),
            ExternalSignInProvider.Telegram => Telegram.IsConfigured(),
            _ => false
        };

    public IReadOnlyList<string> GoogleAudiences()
    {
        if (!Google.Enabled)
            return [];

        return Google.EffectiveClientIds()
            .Where(clientId => clientId.EndsWith(GoogleWebClientSuffix, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
