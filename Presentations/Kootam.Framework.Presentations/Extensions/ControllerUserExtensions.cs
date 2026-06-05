using Kootam.Authentication.Abstractions.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Framework.Presentations.Extensions;

public static class ControllerUserExtensions
{
    public static TUser CurrentUser<TUser>(this ControllerBase controller) where TUser : class
    {
        return controller.HttpContext.RequestServices
            .GetRequiredService<ICurrentUserAccessor>()
            .Get<TUser>();
    }
}