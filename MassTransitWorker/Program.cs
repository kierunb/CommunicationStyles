using MassTransit;
using MassTransit.Worker;
using MassTransit.Worker.Database;
using MassTransit.Worker.Sagas;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

Console.WriteLine(">> Hello, MassTransit Worker!\n");

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

            x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ExistingDbContext<OrderDbContext>();
                        r.UseSqlServer();
                    });


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

        services.AddDbContext<OrderDbContext>(builder =>
        {
            builder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MassTransitSagas;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False", m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                m.MigrationsHistoryTable($"__{nameof(OrderDbContext)}");
            });
        });

        services.AddHostedService<Worker>();
    })
    .Build();

await CreateDatabase(host);

host.Run();

static async Task CreateDatabase(IHost host)
{
    using var scope = host.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    await context.Database.EnsureCreatedAsync();
}