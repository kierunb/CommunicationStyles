using MassTransit.Contracts;

namespace MassTransit.WebAPI.Consumers;

public class PingErrorMessageFaultConsumer : IConsumer<Fault<PingErrorMessage>>
{
    private readonly ILogger<PingErrorMessageFaultConsumer> _logger;

    public PingErrorMessageFaultConsumer(ILogger<PingErrorMessageFaultConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Fault<PingErrorMessage>> context)
    {
        _logger.LogWarning("Ping request failed. Message: {Message}", context.Message.Message);
        _logger.LogError(context.Message.Exceptions?.First()?.Message);
        return Task.CompletedTask;
    }
}

