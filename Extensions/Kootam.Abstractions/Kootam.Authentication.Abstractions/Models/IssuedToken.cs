namespace Kootam.Authentication.Abstractions.Models;

public sealed class IssuedToken<TUserKey>
{
    public required string AccessToken { get; init; }

    public DateTimeOffset AccessTokenExpires { get; init; }

    public RefreshToken<TUserKey>? RefreshToken { get; init; }

    public DateTimeOffset? RefreshTokenExpires { get; init; }
}