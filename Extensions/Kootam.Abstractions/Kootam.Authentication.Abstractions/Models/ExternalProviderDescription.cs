namespace Kootam.Authentication.Abstractions.Models;

public sealed record ExternalProviderDescription(
    string Name,
    string? ClientId,
    string? RedirectUri,
    string? TenantId,
    string? BotId);
