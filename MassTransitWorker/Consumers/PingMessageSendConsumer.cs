using MassTransit.Contracts;

namespace MassTransit.Worker.Consumers;

public class PingMessageSendConsumer : IConsumer<PingMessage>
{
    private readonly ILogger<PingMessageConsumer> _logger;

    public PingMessageSendConsumer(ILogger<PingMessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<PingMessage> context)
    {
        _logger.LogInformation("Consumed PingMessage send with message: {Message}", context.Message.Message);
        return Task.CompletedTask;
    }
}


internal class PingMessageConsumerDefinition : ConsumerDefinition<PingMessageSendConsumer>
{
    public PingMessageConsumerDefinition()
    {
        // override the default endpoint name, for whatever reason
        EndpointName = "ping-queue";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 4;
    }

    //protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    //    IConsumerConfigurator<DiscoveryPingConsumer> consumerConfigurator)
    //{
    //    endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
    //}
}
