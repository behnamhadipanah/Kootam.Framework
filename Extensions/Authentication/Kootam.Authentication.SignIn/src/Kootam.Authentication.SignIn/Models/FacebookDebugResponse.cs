using System.Text.Json.Serialization;

namespace Kootam.Authentication.SignIn.Models;

internal sealed class FacebookDebugResponse
{
    [JsonPropertyName("data")]
    public FacebookDebugData? Data { get; set; }
}
