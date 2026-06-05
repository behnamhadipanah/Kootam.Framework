using Kootam.Extensions.Authentication.Abstractions.CurrentUser;
using Kootam.Extensions.Authentication.CurrentUser;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Extensions.Authentication.DependencyInjection;

public static class CurrentUserServiceCollectionExtensions
{
    public static IServiceCollection AddCurrentUser(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

        return services;
    }
}
