namespace Kootam.Authentication.Abstractions.Models;

public sealed class IssuedToken
{
    public required string AccessToken { get; init; }

    public DateTimeOffset AccessTokenExpires { get; init; }

    public RefreshToken? RefreshToken { get; init; }

    public DateTimeOffset? RefreshTokenExpires { get; init; }
}