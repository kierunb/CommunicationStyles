using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Resilient.Client;

public static class PollyPolicies
{
    public static AsyncRetryPolicy GetRetryPolicy() =>
         Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, retryCount, context) => { Console.WriteLine("Retrying..."); });


    public static Policy GetCircuitBreakerPolicy() =>
   
        Policy
          .Handle<HttpRequestException>()
          .CircuitBreaker(
            exceptionsAllowedBeforeBreaking: 2,
            durationOfBreak: TimeSpan.FromSeconds(20)
          );
    

    public static AsyncCircuitBreakerPolicy GetAdvancedCircuitPolicy() =>
        Policy.Handle<Exception>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(10),
                minimumThroughput: 8,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (ex, ts) => { Console.WriteLine("Circuit breaker opened"); },
                onReset: () => { Console.WriteLine("Circuit breaker reset"); },
                onHalfOpen: () => { Console.WriteLine("Circuit breaker half-opened"); });
    
}


public class PollyRunner
{
    public async Task Execute()
    {
        var policy = PollyPolicies.GetRetryPolicy();

        await policy.ExecuteAsync(async () =>
        {
            await Task.Delay(1000);
            Console.WriteLine("Wrong again :(");
            throw new Exception("Something went wrong");
        });
    }
}
