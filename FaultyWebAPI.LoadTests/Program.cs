using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net.Http;

Console.WriteLine("Hello, NBomber!");
Console.WriteLine("Make sure Faulty.WebAPI is up and running on 'http://localhost:5267'");

var targetUrl = "http://localhost:5267/faulty";
using var httpClient = new HttpClient();

var scenario = Scenario.Create("hello_world_scenario", async context =>
{
    var step1 = await Step.Run("step_1", context, async () =>
    {
        var request =
            Http.CreateRequest("GET", targetUrl);
                //.WithHeader("Accept", "text/html");
                // .WithHeader("Accept", "application/json")
                // .WithBody(new StringContent("{ id: 1 }", Encoding.UTF8, "application/json");
                // .WithBody(new ByteArrayContent(new [] {1,2,3}))


        var response = await Http.Send(httpClient, request);

        return response;
    });

    var step2 = await Step.Run("step_2", context, async () =>
    {
        await Task.Delay(300);
        return Response.Ok(payload: "step_2 response", sizeBytes: 10);
    });

    return !step1.IsError && step2.Payload.Value == "step_2 response"
        ? Response.Ok(statusCode: "200")
        : Response.Fail(statusCode: "500");
})
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 50, during: TimeSpan.FromSeconds(30)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();





// In this example, we configure 
// (RampingInject) - ramp up from 0 to 200 requests per second for 1 minute,
// (Inject) - then we keep the rate of 200 for the next 30 sec.*

//.WithLoadSimulations(
//    Simulation.RampingInject(rate: 200,
//                             interval: TimeSpan.FromSeconds(1),
//                             during: TimeSpan.FromMinutes(1)),

//    Simulation.Inject(rate: 200,
//                      interval: TimeSpan.FromSeconds(1),
//                      during: TimeSpan.FromSeconds(30))