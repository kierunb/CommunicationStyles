using MassTransit;
using MassTransit.Contracts;
using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Console.WriteLine(">> Hello, MassTransit WebAPI!\n");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.SetInMemorySagaRepositoryProvider();

    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

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
        .AddAspNetCoreInstrumentation()
        .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meters
        .AddProcessInstrumentation()                // CPU, RAM, etc.
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