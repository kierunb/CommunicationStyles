using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using System.Net;

namespace Resilient.Client;

public static class ChaosPolicies
{
    public static InjectOutcomePolicy<HttpResponseMessage> GetChaosPolicy()
    {
        // Following example causes the policy to throw SocketException with a probability of 5% if enabled
        var result = new HttpResponseMessage(HttpStatusCode.BadRequest);

        return MonkeyPolicy.InjectResult<HttpResponseMessage>(with =>
            with.Result(result)
                .InjectionRate(0.05)
                .Enabled());
    }   
}


public class ChaosMonkeyRunner
{
    public PolicyResult<HttpResponseMessage> Execute()
    {
        var policy = ChaosPolicies.GetChaosPolicy();

        return policy.ExecuteAndCapture(() =>
        {            
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
    }
}
