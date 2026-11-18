using System.Text.Json.Serialization;

namespace Kootam.Authentication.SignIn.Models;

internal sealed class OidcDiscoveryDocument
{
    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }

    [JsonPropertyName("jwks_uri")]
    public string? JwksUri { get; set; }
}
