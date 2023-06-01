using MassTransit.Contracts;

namespace MassTransit.Worker.Consumers;

public class GettingStartedMessageConsumer : IConsumer<GettingStartedMessage>
{
    private readonly ILogger<GettingStartedMessageConsumer> _logger;

    public GettingStartedMessageConsumer(ILogger<GettingStartedMessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<GettingStartedMessage> context)
    {
        _logger.LogInformation("Message received. Number {Number}, Payload: {Text}", context.Message.Number, context.Message.Payload);
        return Task.CompletedTask;
    }
}
