
namespace MassTransit.Worker.Sagas;

public class InvoiceState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public required string InvoiceNumber { get; set; }

    public required string CurrentState { get; set; }

}

