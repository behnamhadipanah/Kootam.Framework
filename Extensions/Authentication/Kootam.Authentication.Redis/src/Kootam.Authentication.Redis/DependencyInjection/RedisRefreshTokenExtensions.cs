using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.Builder;
using Kootam.Authentication.Redis.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kootam.Authentication.Redis.DependencyInjection;

public static class RedisRefreshTokenExtensions
{

    public static AuthenticationBuilder AddRefreshTokenRedis(this AuthenticationBuilder builder)
    {
       // services.TryAddScoped(typeof(IRefreshTokenRepository<>),typeof(RefreshTokenRepository<>));

       builder.Services.TryAddScoped(typeof(IRefreshTokenService<>),typeof(RedisRefreshTokenService<>));   
        return builder;
    }
    
}