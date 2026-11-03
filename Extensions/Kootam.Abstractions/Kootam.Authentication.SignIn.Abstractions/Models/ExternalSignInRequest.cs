using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.Models;

public sealed record ExternalSignInRequest(
    ExternalSignInProvider Provider,
    string? IdToken = null,
    string? AccessToken = null,
    TelegramLoginData? Telegram = null);
