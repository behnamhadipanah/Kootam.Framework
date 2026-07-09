namespace Kootam.Authentication.Abstractions.Models;

public sealed class IssuedAccessToken
{
    public required string Token { get; init; }

    public required DateTimeOffset Expires { get; init; }
}