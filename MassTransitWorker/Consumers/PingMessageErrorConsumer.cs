using MassTransit.Contracts;
using MassTransit;


public class PingErrorMessageConsumer : IConsumer<PingErrorMessage>
{
    private readonly ILogger<PingMessageConsumer> _logger;

    public PingErrorMessageConsumer(ILogger<PingMessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<PingErrorMessage> context)
    {
        _logger.LogInformation("Processing PingErrorMessage with message: {Message}", context.Message.Message);

        throw new Exception("PingErrorMessageConsumer exception");

        //return Task.CompletedTask;
    }
}
