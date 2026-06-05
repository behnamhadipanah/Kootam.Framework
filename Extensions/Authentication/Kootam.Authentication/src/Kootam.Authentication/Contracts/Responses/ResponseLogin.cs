using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Contracts.Responses;

public record ResponseLogin(string AccessToken,RefreshToken RefreshToken);

