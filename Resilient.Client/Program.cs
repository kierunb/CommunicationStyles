using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Resilient.Client;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// resilient http client 1
builder.Services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
})
.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
{
    TimeSpan.FromSeconds(1),
    TimeSpan.FromSeconds(5),
    TimeSpan.FromSeconds(10)
}));

// resilient http client 2
builder.Services.AddHttpClient("ResilientHttpClient1")
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/http-client", async ([FromServices] IHttpClientFactory httpClientFactory) =>
{
    var targetUrl = "http://localhost:5267/faulty";
    var client = httpClientFactory.CreateClient("ResilientHttpClient1");

    return await client.GetStringAsync(targetUrl);
})
.WithName("HttpClient")
.WithOpenApi();


app.MapGet("/polly-test", async () =>
{
    var pollyRunner = new PollyRunner();

    try
    {
        await pollyRunner.Execute();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    
    return "OK";
})
.WithName("Polly Policies")
.WithOpenApi();

app.MapGet("/simmy-test", () =>
{
    var simmyRunner = new ChaosMonkeyRunner();

    try
    {
        for (int i = 0; i < 100; i++) Console.WriteLine($"Result: {simmyRunner.Execute().Result.StatusCode}");       
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    return "OK";
})
.WithName("Simmy Policies")
.WithOpenApi();

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// delay with Jitter
//var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

//var retryPolicy = Policy
//    .Handle<FooException>()
//    .WaitAndRetryAsync(delay);