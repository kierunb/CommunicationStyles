using MassTransit.Contracts;
using MassTransit.Worker.Activities;

namespace MassTransit.Worker.Sagas;

// Saga is basically a state machine, so we can combine entity with StateMachine behavior driven by messages.
// A saga is also a long-lived transaction managed by a coordinator.
// Sagas are initiated by an event, sagas orchestrate events, and sagas maintain the state of the overall transaction.
// Sagas are designed to manage the complexity of a distributed transaction without locking and immediate consistency.
// They manage state and track any compensations required if a partial failure occurs.

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly ILogger<OrderStateMachine> _logger;

    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        Event(() => AcceptOrder, x =>
        {
            x.OnMissingInstance(
                m => m.ExecuteAsync(
                    context => context.RespondAsync<OrderNotFound>(new { context.Message.OrderId })));
        });
        Event(() => GetOrder, x =>
        {
            x.OnMissingInstance(
                m => m.ExecuteAsync(
                    context => context.RespondAsync<OrderNotFound>(new { context.Message.OrderId })));
        });

        InstanceState(x => x.CurrentState);

        Initially(
            When(SubmitOrder)
                .Then(x => x.Saga.OrderNumber = x.Message.OrderNumber)
                .Then(x => _logger.LogInformation(">> Order {orderId} submitted", x.Message.OrderId))
                .Publish(x => new PingMessage { Message = $"Order {x.Message.OrderId} submitted" })     // or .Send()
                .TransitionTo(Submitted));

        During(Submitted, Accepted,
            When(AcceptOrder)
                .TransitionTo(Accepted)
                .RespondAsync(x => x.Init<Order>(new
                {
                    x.Message.OrderId,
                    x.Saga.OrderNumber,
                    Status = x.Saga.CurrentState
                }))
                .Then(x => _logger.LogInformation(">> Order {orderId} accepted", x.Message.OrderId)));

        DuringAny(
            When(SubmitOrder)
                .Then(x => x.Saga.OrderNumber = x.Message.OrderNumber),
                //.Activity(x => x.OfType<PublishOrderSubmittedActivity>()) // invoke custom activity
            When(GetOrder)
                .RespondAsync(x => x.Init<Order>(new
                {
                    x.Message.OrderId,
                    x.Saga.OrderNumber,
                    Status = x.StateMachine.Accessor.Get(x)
                })));

        // scheduled event

        //Schedule(() => OrderCompletionTimeout, instance => instance.OrderCompletionTimeoutTokenId, s =>
        //{
        //    s.Delay = TimeSpan.FromDays(30);

        //    s.Received = r => r.CorrelateById(context => context.Message.OrderId);
        //});

        _logger = logger;
    }

    public Event<SubmitOrder> SubmitOrder { get; }
    public Event<AcceptOrder> AcceptOrder { get; }
    public Event<GetOrder> GetOrder { get; }

    public State Submitted { get; }
    public State Accepted { get; }
}
