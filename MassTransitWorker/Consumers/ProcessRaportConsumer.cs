using MassTransit.Contracts;

namespace MassTransit.Worker.Consumers;

public class ProcessRaportConsumer : IConsumer<ProcessRaportCommand>
{
    private readonly ILogger<ProcessRaportConsumer> _logger;

    public ProcessRaportConsumer(ILogger<ProcessRaportConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessRaportCommand> context)
    {
        _logger.LogInformation("Raport processing started. RaportId {RaportId}, RaportName: {RaportName}", 
            context.Message.RaportId, context.Message.RaportName);
        
        await Task.Delay(5000); // simulate long running process

        _logger.LogInformation("Raport sucesfully processed. RaportId {RaportId}, RaportName: {RaportName}", 
            context.Message.RaportId, context.Message.RaportName);

        await context.Publish(new RaportProcessedEvent { RaportId = context.Message.RaportId, RaportStatus = "Completed" });
    }
}
