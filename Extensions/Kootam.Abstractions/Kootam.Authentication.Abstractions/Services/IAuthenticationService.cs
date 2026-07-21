using Kootam.Authentication.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Services;

public interface IAuthenticationService<TUserKey>
{
    Task SignInAsync(IssuedToken<TUserKey> token,CancellationToken cancellationToken=default);

    Task SignOutAsync(CancellationToken cancellationToken=default);
}