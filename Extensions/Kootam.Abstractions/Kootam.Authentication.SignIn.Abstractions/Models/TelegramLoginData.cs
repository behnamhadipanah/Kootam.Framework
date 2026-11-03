namespace Kootam.Authentication.SignIn.Models;

public sealed record TelegramLoginData(
    long Id,
    string? FirstName,
    string? LastName,
    string? Username,
    string? PhotoUrl,
    long AuthDate,
    string Hash);
