using MassTransit.Contracts;
using MassTransit.Contracts.InvoiceStateMachine;

namespace MassTransit.Worker.Sagas;

public class InvoiceStateMachine : MassTransitStateMachine<InvoiceState>
{
    private readonly ILogger<InvoiceStateMachine> _logger;

    public InvoiceStateMachine(ILogger<InvoiceStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);

        // Events
        Event(() => SubmitInvoice, e => e.CorrelateById(cxt => cxt.Message.InvoiceId));
        Event(() => AcceptInvoice, e => e.CorrelateById(cxt => cxt.Message.InvoiceId));
        Event(() => FinalizeInvoice, e => e.CorrelateById(cxt => cxt.Message.InvoiceId));
        Event(() => GetInvoice, e => e.CorrelateById(cxt => cxt.Message.InvoiceId));


        // Flow / Behavior
        Initially(
            When(SubmitInvoice)
                .Then(x => _logger.LogInformation(">>> Event: 'Submit Invoice' Invoice: {invoiceNumber} ", x.Message.InvoiceNumber))
                .Then(x => x.Saga.InvoiceNumber = x.Message.InvoiceNumber)
                .Publish(x => new PingMessage { Message = $"Invoice: {x.Saga.InvoiceNumber} submitted." })
                .Then(x => _logger.LogInformation(">>> Event: 'Submit Invoice'. Transition from 'Initial' to: 'Submitted' state."))
                .TransitionTo(Submitted));

        During(Submitted, Accepted, 
            When(AcceptInvoice)
                .Then(x => _logger.LogInformation(">>> Event: 'Accept Invoice' Invoice: {invoiceNumber} ", x.Message.InvoiceNumber))
                .Then(x => _logger.LogInformation(">>> Event: 'Accept Invoice'. Transition from 'Submitted' to: 'Accepted' state."))
                .TransitionTo(Accepted));

        During(Accepted,
            When(FinalizeInvoice)
                .Then(x => _logger.LogInformation(">>> Event: 'Finalize Invoice' Invoice: {invoiceNumber} ", x.Message.InvoiceNumber))
                .Then(x => _logger.LogInformation(">>> Event: 'Finalize Invoice'. Transition from 'Accepted' to: 'Finalized' state."))
                .Finalize());

        DuringAny(
            When(GetInvoice)
                .Then(x => _logger.LogInformation(">>> Event: 'Get Invoice' Invoice: {invoiceNumber} ", x.Message.InvoiceNumber))
                    .RespondAsync(x => x.Init<CurrentInvoiceState>(new
                    {
                        x.Message.InvoiceId,
                        x.Saga.InvoiceNumber,
                        Status = x.StateMachine.Accessor.Get(x)
                    }))); 
    }


    #region States
    
    // Initial and Final states are defined by defaul
    public State Submitted { get; private set; }
    public State Accepted { get; private set; } 
    
    #endregion


    #region Events

    public Event<SubmitInvoice> SubmitInvoice { get; private set; }
    public Event<AcceptInvoice> AcceptInvoice { get; private set; }
    public Event<FinalizeInvoice> FinalizeInvoice { get; private set; }
    public Event<GetInvoice> GetInvoice { get; private set; }
    
    #endregion
}
