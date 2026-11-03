namespace Kootam.Authentication.SignIn.Models;

public sealed record OidcTokenDocument(
    string Subject,
    string? Email,
    bool EmailVerified,
    string? GivenName,
    string? FamilyName);
