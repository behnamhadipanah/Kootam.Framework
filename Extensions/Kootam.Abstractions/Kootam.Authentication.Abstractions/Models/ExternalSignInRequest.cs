using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.Abstractions.Models;

public sealed record ExternalSignInRequest(
    ExternalSignInProvider Provider,
    string? IdToken = null,
    string? AccessToken = null,
    TelegramLoginData? Telegram = null);
