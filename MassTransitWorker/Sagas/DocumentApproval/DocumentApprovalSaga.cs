using MassTransit.Contracts;
using MassTransit.Contracts.DocumentApprovalEvents;

namespace MassTransit.Worker.Sagas.DocumentApproval;



public class DocumentApprovalSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }                 // Saga Correlation Id

    public required string CurrentState { get; set; }       // Saga Current State

    public required string DocumentName { get; set; } = default!; // Document Number
    public  bool Decision { get; set; }

}
  



public class DocumentApprovalSaga : MassTransitStateMachine<DocumentApprovalSagaState>
{
    private readonly ILogger<DocumentApprovalSaga> _logger;

    public DocumentApprovalSaga(ILogger<DocumentApprovalSaga> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState); // which state propery stores custate of saga/process    
                                            // by default, saga has two states: Initial and Final

        // Events
        Event(() => DocumentSubmitted, e => e.CorrelateById(cxt => cxt.Message.DocumentId));
        Event(() => DocumentAccepted, e => e.CorrelateById(cxt => cxt.Message.DocumentId));


        // Flow / Behavior
        Initially(
            When(DocumentSubmitted)
                .Then(x => x.Saga.DocumentName = x.Message.DocumentName)
                .Then(x => _logger.LogInformation(">>> Event: 'Document Submitted' Document: {documentNumber}", x.Message.DocumentId))
                
                //.Send()
                .Publish(x => new RaportProcessedEvent { RaportId = x.Message.DocumentId, RaportStatus = "Submitted"  })
                // Request/Response
                // Schedule
                // HandleDocumentSubmission()
                //.Activity(x => x.OfType<OrderClosedActivity>())
                //.Respond
                .TransitionTo(Submitted));

        During(Submitted,
            When(DocumentAccepted)
                    .Then(x => x.Instance.Decision = true)
                    .Then(x => _logger.LogInformation(">>> Event: 'Document Accepted' Document: {documentNumber}", x.Message.DocumentId))
                    .TransitionTo(Approved));


        DuringAny(
            When(DocumentSubmittedFaulted)
                .Then(x => _logger.LogError(">>> Event: 'Document Submitted' Faulted. Document: {documentNumber}", x.Message.Message.DocumentId))
                .TransitionTo(Final));
    }


    // Saga States
    public State Submitted { get; private set; }
    public State Approved { get; private set; }


    // Events

    public Event<DocumentSubmitted> DocumentSubmitted { get; private set; }
    public Event<Fault<DocumentSubmitted>> DocumentSubmittedFaulted { get; private set; }
    public Event<DocumentAccepted> DocumentAccepted { get; private set; }
}

// 