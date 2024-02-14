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


        // Flow / Behavior
        Initially(
            When(SubmitInvoice)
                .Then(x => x.Saga.InvoiceNumber = x.Message.InvoiceNumber)
                .Then(x => _logger.LogInformation(">>> Invoice {invoiceNumber} submitted", x.Message.InvoiceNumber))
                .TransitionTo(Submitted));

        During(Submitted, Accepted, 
            When(AcceptInvoice)
                .Then(x => _logger.LogInformation(">>> Invoice {invoiceNumber} accepted", x.Message.InvoiceNumber))
                .TransitionTo(Accepted));
    }

    #region States
    public State Submitted { get; private set; }
    public State Accepted { get; private set; } 
    #endregion


    #region Events
    public Event<SubmitInvoice> SubmitInvoice { get; private set; }
    public Event<AcceptInvoice> AcceptInvoice { get; private set; }
    #endregion
}
