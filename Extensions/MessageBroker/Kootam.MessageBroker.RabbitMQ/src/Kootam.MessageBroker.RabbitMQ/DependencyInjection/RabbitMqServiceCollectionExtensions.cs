using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.RabbitMQ.Connection;
using Kootam.MessageBroker.RabbitMQ.Options;
using Kootam.MessageBroker.RabbitMQ.Publisher;
using Kootam.MessageBroker.RabbitMQ.Serialization;
using Kootam.MessageBroker.RabbitMQ.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.MessageBroker.RabbitMQ.DependencyInjection;

public static class RabbitMqServiceCollectionExtensions
{
    /// <summary>
    /// Registers RabbitMQ services.
    /// Exposes: IMessagePublisher, IMessageSubscriber, IMessageBus, IMessageBrokerConnection.
    /// </summary>
    public static IServiceCollection AddRabbitMQ(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = RabbitMqOptions.SectionName)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(sectionName));
        return services.AddRabbitMQCore();
    }

    /// <summary>
    /// Registers RabbitMQ services with an inline options action.
    /// </summary>
    public static IServiceCollection AddRabbitMQ(
        this IServiceCollection services,
        Action<RabbitMqOptions> configure)
    {
        services.Configure(configure);
        return services.AddRabbitMQCore();
    }

    private static IServiceCollection AddRabbitMQCore(this IServiceCollection services)
    {
        // Infrastructure
        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<IMessageBrokerConnection>(sp => sp.GetRequiredService<RabbitMqConnection>());
        services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();

        // Publisher & Subscriber (singleton — one channel pool per process is fine)
        services.AddSingleton<RabbitMqPublisher>();
        services.AddSingleton<RabbitMqSubscriber>();

        // Named interfaces
        services.AddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<RabbitMqPublisher>());
        services.AddSingleton<IMessageSubscriber>(sp => sp.GetRequiredService<RabbitMqSubscriber>());

        // Combined bus
        services.AddSingleton<IMessageBus, RabbitMqBus>();

        return services;
    }
}