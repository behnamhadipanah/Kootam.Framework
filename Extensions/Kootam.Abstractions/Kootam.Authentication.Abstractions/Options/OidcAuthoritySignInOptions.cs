namespace Kootam.Authentication.Abstractions.Options;

public sealed class OidcAuthoritySignInOptions : OidcSignInOptions
{
    public string? Authority { get; set; }

    public string? Issuer()
    {
        if (string.IsNullOrWhiteSpace(Authority))
            return null;

        var trimmed = Authority.Trim().TrimEnd('/');
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            return null;

        if (uri.Scheme is not "https" and not "http")
            return null;

        if (!string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
            return null;

        return trimmed;
    }

    public override bool IsConfigured()
        => base.IsConfigured() && Issuer() is not null;
}
