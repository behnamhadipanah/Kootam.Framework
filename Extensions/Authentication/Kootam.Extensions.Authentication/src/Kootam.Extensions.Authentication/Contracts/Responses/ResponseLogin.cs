using Kootam.Extensions.Authentication.Abstractions.Models;

namespace Kootam.Extensions.Authentication.Contracts.Responses;

public record ResponseLogin(string AccessToken,RefreshToken RefreshToken);

