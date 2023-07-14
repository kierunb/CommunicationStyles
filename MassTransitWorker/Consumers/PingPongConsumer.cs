using MassTransit.Contracts;

namespace MassTransit.Worker.Consumers;

public class PingPongConsumer : IConsumer<PingRequest>
{
    private readonly ILogger<PingPongConsumer> _logger;

    public PingPongConsumer(ILogger<PingPongConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PingRequest> context)
    {
        _logger.LogInformation("Consumed PingMessage with message: {Message}", context.Message.Message);
        await context.RespondAsync(new PingResponse { ResponseMessage = $"Pong: {context.Message.Message}" });
    }
}
