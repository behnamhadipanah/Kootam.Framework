using FluentValidation;
using Kootam.Cqrs.Abstractions.Behaviors;
using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Kootam.Cqrs.Behaviors;
using Kootam.Cqrs.Commands;
using Kootam.Cqrs.Options;
using Kootam.Cqrs.Queries;

namespace Kootam.Cqrs.DependencyInjections;

public static class CqrsServiceCollectionExtensions
{
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Action<CqrsOptions>? configure = null)
    {
        var options = new CqrsOptions();
        configure(options);


        RegisterHandlers(services, options.Assemblies);
        RegisterValidators(services, options.Assemblies);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainExceptionBehavior<,>));




        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();


        return services;
    }


    private static void RegisterHandlers(IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && !t.IsInterface);
        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType &&
                (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))));


            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, type);
            }
        }
    }


    private static void RegisterValidators(IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var validatorType = typeof(IValidator<>);

        var validatorTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.IsGenericType &&
                                                  i.GetGenericTypeDefinition() == validatorType));

        foreach (var validator in validatorTypes)
        {
            var interfaces = validator.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType);

            foreach (var @interface in interfaces)
            {
                services.AddTransient(@interface, validator);
            }
        }
    }
}