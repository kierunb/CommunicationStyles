using MassTransit.Contracts;
using MassTransit.Worker.Sagas;

namespace MassTransit.Worker.Activities;

public class PublishOrderSubmittedActivity :
    IStateMachineActivity<OrderState, SubmitOrder>
{
    readonly IOrderSubmittedService _service;

    public PublishOrderSubmittedActivity(IOrderSubmittedService service)
    {
        _service = service;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("publish-order-submitted");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderState, SubmitOrder> context, IBehavior<OrderState, SubmitOrder> next)
    {
        await _service.OnOrderSubmitted(context.Saga.CorrelationId);

        // always call the next activity in the behavior
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderState, SubmitOrder, TException> context,
        IBehavior<OrderState, SubmitOrder> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}

