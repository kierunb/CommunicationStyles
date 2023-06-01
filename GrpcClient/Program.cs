using Grpc.Net.Client;
using GrpcService;

Console.WriteLine("Hello, Grpc Client!");

var channel = GrpcChannel.ForAddress("http://localhost:5041");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine($"Hello: {reply}");

Console.ReadLine();
