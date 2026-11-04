namespace Kootam.Authentication.Abstractions.Options;

public sealed class TelegramSignInOptions
{
    public bool Enabled { get; set; }

    public string? BotToken { get; set; }

    public string? RedirectUri { get; set; }

    public bool IsConfigured()
        => Enabled && !string.IsNullOrWhiteSpace(BotToken);
}
