using MassTransit.Contracts;

namespace MassTransit.WebAPI.Consumers;

public class RaportProcessedEventConsumer : IConsumer<RaportProcessedEvent>
{
    private readonly ILogger<RaportProcessedEventConsumer> _logger;

    public RaportProcessedEventConsumer(ILogger<RaportProcessedEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RaportProcessedEvent> context)
    {
        _logger.LogInformation("Raport processed. RaportId {RaportId}, RaportStatus: {RaportStatus}", 
            context.Message.RaportId, context.Message.RaportStatus);
        
        await Task.CompletedTask;    
    }
}
