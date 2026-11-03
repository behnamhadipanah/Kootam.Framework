namespace Kootam.Authentication.SignIn.Contracts.Options;

public sealed class MicrosoftSignInOptions : OidcSignInOptions
{
    public string Tenant { get; set; } = "common";
}
