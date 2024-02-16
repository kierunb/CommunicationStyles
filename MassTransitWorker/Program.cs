using MassTransit;
using MassTransit.Logging;
using MassTransit.Worker;
using MassTransit.Worker.Activities;
using MassTransit.Worker.Consumers;
using MassTransit.Worker.Database;
using MassTransit.Worker.Sagas;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;

Console.WriteLine(">> Hello, MassTransit Worker!\n");

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

// OpenTelemtry
Sdk.CreateTracerProviderBuilder()
    .ConfigureResource(r =>
        r.AddService("MassTransit.Worker",
            serviceVersion: "1.0",
            serviceInstanceId: Environment.MachineName))
    //.AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
    .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
                                                      //.AddConsoleExporter() // Any OTEL suportable exporter can be used here
    .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); })   // for jeager with OLTP endpoint
    .Build();

// TODO: Add Metrics and Prometheus exporter

string sagaStateDbConnString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MassTransitSagasState;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
var entryAssembly = Assembly.GetEntryAssembly();

try
{
    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                //x.SetInMemorySagaRepositoryProvider();

                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                        .EntityFrameworkRepository(r =>
                        {
                            r.ExistingDbContext<OrderDbContext>();
                            r.UseSqlServer();
                        });

                // Invoice Saga Configuration

                //x.AddSagaStateMachine<InvoiceStateMachine, InvoiceState>().InMemoryRepository();
                x.AddSagaStateMachine<InvoiceStateMachine, InvoiceState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ExistingDbContext<InvoiceDbContext>();
                        r.UseSqlServer();
                    });


                x.AddConsumer<PingMessageSendConsumer>().Endpoint(e => e.Name = "ping-queue");
                //x.AddConsumer<PingMessageSendConsumer, PingMessageConsumerDefinition>();

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
                builder.UseSqlServer(sagaStateDbConnString, m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{nameof(OrderDbContext)}");
                });
            });

            services.AddDbContext<InvoiceDbContext>(builder =>
            {
                builder.UseSqlServer(sagaStateDbConnString, m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{nameof(InvoiceDbContext)}");
                });
            });


            services.AddTransient<IOrderSubmittedService, OrderSubmittedService>();

            services.AddHostedService<Worker>();
        })
    .UseSerilog()
    .Build();

    await CreateDatabases(host);

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}



static async Task CreateDatabases(IHost host)
{
    using var scope = host.Services.CreateScope();

    var contextOrdersDb = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    var contextInvoicesDb = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

    //await contextOrdersDb.Database.EnsureCreatedAsync();
    //await contextInvoicesDb.Database.EnsureCreatedAsync();
    await contextOrdersDb.Database.MigrateAsync();
    await contextInvoicesDb.Database.MigrateAsync();

}