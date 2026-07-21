using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Contracts.Responses;

public record ResponseLogin<TUserKey>(string AccessToken,RefreshToken<TUserKey> RefreshToken);

