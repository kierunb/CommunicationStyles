using Polly.Registry;

namespace Resilient.Client;

public static class AppPolicyRegistry
{
    public static void ConfigurePollyPolicies(this IServiceCollection services)
    {
        PolicyRegistry registry = new()
        {
            //{ "RepositoryResilienceStrategy", /* define Policy or PolicyWrap strategy */ },
            //{ "CollaboratingMicroserviceResilienceStrategy", /* define Policy or PolicyWrap strategy */ },
            //{ "ThirdPartyApiResilienceStrategy", /* define Policy or PolicyWrap strategy */ }
            /* etc */
        };

        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
    }
}
