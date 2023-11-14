using MassTransit.Contracts;
using MassTransit;


public class PingMessageBatchConsumer : IConsumer<Batch<PingMessage>>
{
    private readonly ILogger<PingMessageConsumer> _logger;

    public PingMessageBatchConsumer(ILogger<PingMessageConsumer> logger)
    {
        _logger = logger;
    }


    // MassTransit supports receiving multiple messages and delivering those messages to the consumer in a batch.
    // https://masstransit.io/documentation/concepts/consumers#batch-consumers
    public Task Consume(ConsumeContext<Batch<PingMessage>> context)
    {
        _logger.LogInformation("PingMessage Batch started");

        for (int i = 0; i < context.Message.Length; i++)
        {
            ConsumeContext<PingMessage> message = context.Message[i];
            _logger.LogInformation("Consumed PingMessage with message: {Message}", message.Message);
        }

        _logger.LogInformation("PingMessage Batch completed");

        return Task.CompletedTask;
    }
}