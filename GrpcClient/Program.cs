using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcService;

Console.WriteLine("Hello, Grpc Client!");

var defaultMethodConfig = new MethodConfig
{
    Names = { MethodName.Default },
    RetryPolicy = new RetryPolicy
    {
        MaxAttempts = 5,
        InitialBackoff = TimeSpan.FromSeconds(1),
        MaxBackoff = TimeSpan.FromSeconds(5),
        BackoffMultiplier = 1.5,
        RetryableStatusCodes = { StatusCode.Unavailable }
    }
};

// non-resilient channel
//var channel = GrpcChannel.ForAddress("http://localhost:5041");

// resilient channel
var channel = GrpcChannel.ForAddress("http://localhost:5041", new GrpcChannelOptions
{
    ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
});

var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine($"Hello: {reply}");

Console.ReadLine();
