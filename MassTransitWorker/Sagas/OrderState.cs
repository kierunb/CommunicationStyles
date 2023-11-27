namespace MassTransit.Worker.Sagas;

public class OrderState : SagaStateMachineInstance
{
    public required string CurrentState { get; set; }

    public required string OrderNumber { get; set; }

    public Guid CorrelationId { get; set; }
}
