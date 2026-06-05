using Kootam.Authentication.Abstractions.CurrentUser;
using Kootam.Authentication.CurrentUser;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

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
