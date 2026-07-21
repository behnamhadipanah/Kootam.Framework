using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.SqlServer.Context;
using Kootam.Authentication.SqlServer.Repositories;
using Kootam.Authentication.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kootam.Authentication.SqlServer.DependencyInjection;

public static class RefreshTokenSqlExtensions
{
    public static IServiceCollection AddRefreshTokenSql(
        this IServiceCollection services)
        
    {
        services.TryAddScoped(typeof(IRefreshTokenRepository<>),typeof(RefreshTokenRepository<>));

        services.TryAddScoped(typeof(IRefreshTokenService<>),typeof(SqlRefreshTokenService<>));

        return services;
    }
}