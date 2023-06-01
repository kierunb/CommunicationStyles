using MassTransit;
using MassTransit.Worker;
using System.Reflection;

var assembly = Assembly.GetEntryAssembly();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            // By default, sagas are in-memory, but should be changed to a durable
            // saga repository.
            x.SetInMemorySagaRepositoryProvider();

            var entryAssembly = Assembly.GetEntryAssembly();

            x.AddConsumers(entryAssembly);
            x.AddSagaStateMachines(entryAssembly);
            x.AddSagas(entryAssembly);
            x.AddActivities(entryAssembly);

            // In-memory provider
            //x.UsingInMemory((context, cfg) =>
            //{
            //    cfg.ConfigureEndpoints(context);
            //});

            // RabbitMQ provider
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });


        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
