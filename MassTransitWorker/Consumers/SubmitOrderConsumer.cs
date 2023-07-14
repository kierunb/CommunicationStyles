using MassTransit.Contracts;

namespace MassTransit.Worker.Consumers;

public class SubmitOrderConsumer : IConsumer<PingMessage>
{
    private readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<PingMessage> context)
    {
        _logger.LogInformation("Consumed PingMessage with id: {Id} and message {Message}", 
            context.Message.PingId, 
            context.Message.Message);

        return Task.CompletedTask;
    }
}
