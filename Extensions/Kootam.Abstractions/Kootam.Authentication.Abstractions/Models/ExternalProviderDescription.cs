namespace Kootam.Authentication.SignIn.Models;

public sealed record ExternalProviderDescription(
    string Name,
    string? ClientId,
    string? RedirectUri,
    string? TenantId,
    string? BotId);
