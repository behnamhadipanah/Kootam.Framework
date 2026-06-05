using Kootam.MessageBroker.RabbitMQ.DependencyInjection;
using Kootam.MessageBroker.RabbitMQ.Sample;
using Kootam.MessageBroker.RabbitMQ.Sample.Workers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMQ(ctx.Configuration);

        // services.AddRabbitMQ(opt =>
        // {
        //     opt.HostName = "localhost";
        //     opt.UserName = "smartpulseAdmin";
        //     opt.Password  = "Sm@rtM0ney";
        // });

        services.AddHostedService<OrderPublisherWorker>();
        services.AddHostedService<OrderSubscriberWorker>();
    })
    .Build();

host.RunAsync();