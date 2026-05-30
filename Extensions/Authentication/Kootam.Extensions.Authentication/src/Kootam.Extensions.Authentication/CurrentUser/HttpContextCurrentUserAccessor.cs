using Kootam.Extensions.Authentication.Abstractions.CurrentUser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Extensions.Authentication.CurrentUser;

public class HttpContextCurrentUserAccessor(
        IHttpContextAccessor contextAccessor,
        IServiceProvider serviceProvider) : ICurrentUserAccessor
{
    public TUser Get<TUser>() where TUser : class
    {
        var principal = contextAccessor.HttpContext?.User;

        if (principal == null)
            return default;

        var mapper = serviceProvider
            .GetRequiredService<ICurrentUserMapper<TUser>>();

        return mapper.Map(principal);
    }
}
