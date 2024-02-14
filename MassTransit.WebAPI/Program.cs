using MassTransit;
using MassTransit.Contracts;
using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;

Console.WriteLine(">> Hello, MassTransit WebAPI!\n");

Serilog.Debugging.SelfLog.Enable(Console.Error);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();



try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(entryAssembly);

    builder.Services.AddMassTransit(x =>
    {
        x.SetKebabCaseEndpointNameFormatter();

        x.SetInMemorySagaRepositoryProvider();

        x.AddConsumers(entryAssembly);
        x.AddSagaStateMachines(entryAssembly);
        x.AddSagas(entryAssembly);
        x.AddActivities(entryAssembly);

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h => {
                h.Username("guest");
                h.Password("guest");
            });

            // error handling and reliability settings:
            cfg.UseMessageRetry(r => {
                r.Interval(retryCount: 2, interval: 100);
                r.Ignore(typeof(StackOverflowException));
            });

            // redelivery at later time (reqire delayed-exchange plugin):
            cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));

            // outbox pattern to buffer messages until the consumer completes successfully
            //cfg.UseInMemoryOutbox(context);

            cfg.ConfigureEndpoints(context);
        });
    });

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r =>
            r.AddService("MassTransit.WebAPI",
                serviceVersion: "1.0",
                serviceInstanceId: Environment.MachineName))
        .WithTracing(b => b
            .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
            .AddAspNetCoreInstrumentation()
            //.AddConsoleExporter()
            .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); }))   // for jeager with OLTP endpoint
         .WithMetrics(metrics => metrics
            .AddProcessInstrumentation()                // CPU, RAM, etc.
            .AddRuntimeInstrumentation()                // .NET runtime metrics - GC, ThreadPool, etc.
            .AddAspNetCoreInstrumentation()
            .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meters
                                                        //.AddConsoleExporter()
            .AddPrometheusExporter());

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /// Endpoints

    app.MapGet("/", () => "Hello in MassTransit Web Api Client. Visit '/swagger' for list of endpints.");

    app.MapGet("/send-message", async (IBus bus) =>
    {
        await bus.Publish(new GettingStartedMessage { Number = DateTime.Now.Second, Payload = $"The time is {DateTimeOffset.Now}" });
        return "GettingStartedMessage sent.";
    })
    .WithName("Send Message")
    .WithOpenApi();

    app.MapControllers();

    app.UseOpenTelemetryPrometheusScrapingEndpoint();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

