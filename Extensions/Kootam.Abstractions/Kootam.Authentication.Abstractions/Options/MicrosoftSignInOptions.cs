namespace Kootam.Authentication.Abstractions.Options;

public sealed class MicrosoftSignInOptions : OidcSignInOptions
{
    public string Tenant { get; set; } = "common";
}
