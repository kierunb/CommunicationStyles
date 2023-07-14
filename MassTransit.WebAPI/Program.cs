using MassTransit;
using MassTransit.Contracts;

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

app.Run();