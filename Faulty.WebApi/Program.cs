using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapGet("/faulty", () =>
{
    var randomNumber = Random.Shared.Next(1, 100);

    if (randomNumber % 2 == 0) throw new Exception("Something went wrong");
    
    return "OK";
})
.WithName("Faulty")
.WithOpenApi();

app.MapGet("/not-faulty", () =>
{
    return "OK";
})
.WithName("Not-faulty")
.WithOpenApi();

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/limited", () => Results.Ok($"Hello {GetTicks()}"))
.WithName("Limited")
.WithOpenApi()
.RequireRateLimiting("fixed");

app.Run();
