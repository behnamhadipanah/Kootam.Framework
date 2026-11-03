namespace Kootam.Authentication.SignIn.Models;

public sealed record ContactOtpRateEntry(int Count, DateTimeOffset WindowStartedAt);
