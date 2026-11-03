using System.Text.Json.Serialization;

namespace Kootam.Authentication.SignIn.Models;

internal sealed class FacebookDebugData
{
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }
}
