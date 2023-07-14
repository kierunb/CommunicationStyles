using MassTransit.Contracts;
using MassTransit;


public class PingMessageConsumer : IConsumer<PingMessage>
{
    private readonly ILogger<PingMessageConsumer> _logger;

    public PingMessageConsumer(ILogger<PingMessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<PingMessage> context)
    {
        _logger.LogInformation("Consumed PingMessage with message: {Message}", context.Message.Message);
        return Task.CompletedTask;
    }
}
