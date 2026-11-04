using Kootam.Authentication.Abstractions.Options;

namespace Kootam.Authentication.SignIn.Configuration;

public static class SignInConfigurationRules
{
    public static IEnumerable<string> ExternalWarnings(ExternalSignInOptions external)
    {
        if (external.Google.Enabled && external.GoogleAudiences().Count == 0)
        {
            yield return
                "Google sign-in is enabled but SignIn:External:Google:ClientId is missing or is not a Web client id ending in .apps.googleusercontent.com.";
        }

        foreach (var clientId in external.Google.EffectiveClientIds())
        {
            if (clientId.StartsWith("GOCSPX-", StringComparison.Ordinal))
            {
                yield return
                    "SignIn:External:Google:ClientId looks like a client secret. Use the Web client id ending in .apps.googleusercontent.com.";
            }
        }

        if (external.Apple.Enabled && !external.Apple.IsConfigured())
            yield return "Apple sign-in is enabled but SignIn:External:Apple:ClientId is required.";

        if (external.Microsoft.Enabled && !external.Microsoft.IsConfigured())
            yield return "Microsoft sign-in is enabled but SignIn:External:Microsoft:ClientId is required.";

        if (external.Facebook.Enabled && !external.Facebook.IsConfigured())
            yield return "Facebook sign-in is enabled but AppId and AppSecret are required.";

        if (external.Telegram.Enabled && !external.Telegram.IsConfigured())
            yield return "Telegram sign-in is enabled but SignIn:External:Telegram:BotToken is required.";
    }
}
