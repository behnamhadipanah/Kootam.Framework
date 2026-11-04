namespace Kootam.Authentication.Abstractions.Options;

public sealed class FacebookSignInOptions
{
    public bool Enabled { get; set; }

    public string? AppId { get; set; }

    public string? AppSecret { get; set; }

    public string? RedirectUri { get; set; }

    public bool IsConfigured()
        => Enabled
           && !string.IsNullOrWhiteSpace(AppId)
           && !string.IsNullOrWhiteSpace(AppSecret);
}
