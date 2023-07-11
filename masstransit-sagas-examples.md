# MassTransit Sagas Examples

## Sample 1

```csharp
using EventBus.Constants;
using EventBus.Events;
using EventBus.Events.Interfaces;
using EventBus.Messages;
using EventBus.Messages.Interfaces;
using MassTransit;
using SagaOrchestrationStateMachine.StateInstances;
using ILogger = Serilog.ILogger;

namespace SagaOrchestrationStateMachine.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    private readonly ILogger _logger;

    // Commands
    private Event<ICreateOrderMessage> CreateOrderMessage { get; set; }

    // Events
    private Event<IStockReservedEvent> StockReservedEvent { get; set; }
    private Event<IStockReservationFailedEvent> StockReservationFailedEvent { get; set; }
    private Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
    private Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }

    // States
    private State OrderCreated { get; set; }
    private State StockReserved { get; set; }
    private State StockReservationFailed { get; set; }
    private State PaymentCompleted { get; set; }
    private State PaymentFailed { get; set; }

    public OrderStateMachine() //ILogger<OrderStateMachine> logger)
    {
        _logger = Serilog.Log.Logger; 
        InstanceState(x => x.CurrentState);

        Event(() => CreateOrderMessage, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
            .SelectId(context => Guid.NewGuid()));
        Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
        Event(() => StockReservationFailedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

        Initially(
            When(CreateOrderMessage)
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("CreateOrderMessage received in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .Then(context =>
                {
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CreatedDate = DateTime.UtcNow;
                    context.Saga.PaymentAccountId = context.Message.PaymentAccountId;
                    context.Saga.TotalPrice = context.Message.TotalPrice;
                })
                .Publish(
                    context => new OrderCreatedEvent
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        OrderItemList = context.Message.OrderItemList
                    })
                .TransitionTo(OrderCreated)
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("OrderCreatedEvent published in OrderStateMachine: {ContextSaga} ", context.Saga); }));

        During(OrderCreated,
            When(StockReservedEvent)
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("StockReservedEvent received in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue:{QueuesConsts.CompletePaymentMessageQueueName}"),
                    context => new CompletePaymentMessage 
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        TotalPrice = context.Saga.TotalPrice,
                        CustomerId = context.Saga.CustomerId,
                        OrderItemList = context.Message.OrderItemList
                    })
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("CompletePaymentMessage sent in OrderStateMachine: {ContextSaga} ", context.Saga); }),
            When(StockReservationFailedEvent)
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("StockReservationFailedEvent received in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .TransitionTo(StockReservationFailed)
                .Publish(
                    context => new OrderFailedEvent
                    {
                        OrderId = context.Saga.OrderId,
                        CustomerId = context.Saga.CustomerId,
                        ErrorMessage = context.Message.ErrorMessage
                    })
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("OrderFailedEvent published in OrderStateMachine: {ContextSaga} ", context.Saga); })
        );

        During(StockReserved,
            When(PaymentCompletedEvent)
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("PaymentCompletedEvent received in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .TransitionTo(PaymentCompleted)
                .Publish(
                    context => new OrderCompletedEvent
                    {
                        OrderId = context.Saga.OrderId,
                        CustomerId = context.Saga.CustomerId
                    })
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("OrderCompletedEvent published in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .Finalize(),
            When(PaymentFailedEvent)
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("PaymentFailedEvent received in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .Publish(context => new OrderFailedEvent
                {
                    OrderId = context.Saga.OrderId,
                    CustomerId = context.Saga.CustomerId,
                    ErrorMessage = context.Message.ErrorMessage
                })
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("OrderFailedEvent published in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .Send(new Uri($"queue:{QueuesConsts.StockRollBackMessageQueueName}"),
                    context => new StockRollbackMessage
                    {
                        OrderItemList = context.Message.OrderItemList
                    })
                .Then(context => { _logger.ForContext("CorrelationId", context.Saga.CorrelationId).Information("StockRollbackMessage sent in OrderStateMachine: {ContextSaga} ", context.Saga); })
                .TransitionTo(PaymentFailed)
        );
    }
}
```

## Sample 2 

```csharp
using MassTransit;
using StateMachineSample.Events;

namespace StateMachineSample.StateMachine;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        #region EventsDefinitions

        Event(() => OrderProcessInitializationEvent);
        Event(() => OrderProcessInitializationFaultEvent,
            x => x.CorrelateById(context => context.InitiatorId ?? context.Message.Message.OrderId));

        Event(() => CheckProductStockEvent);
        Event(() => CheckProductStockFaultEvent,
            x => x.CorrelateById(context => context.InitiatorId ?? context.Message.Message.OrderId));

        Event(() => TakePaymentEvent);
        Event(() => TakePaymentEventFaultEvent,
            x => x.CorrelateById(context => context.InitiatorId ?? context.Message.Message.OrderId));

        Event(() => CreateOrderEvent);
        Event(() => CreateOrderFaultEvent,
            x => x.CorrelateById(context => context.InitiatorId ?? context.Message.Message.OrderId));

        Event(() => OrderProcessFailedEvent);

        #endregion


        InstanceState(x => x.CurrentState);

        #region Flow

        During(Initial,
            When(OrderProcessInitializationEvent)
                .Then(x => x.Saga.OrderStartDate = DateTime.Now)
                .TransitionTo(OrderProcessInitializedState));

        During(OrderProcessInitializedState,
            When(CheckProductStockEvent)
                .TransitionTo(CheckProductStockState));

        During(CheckProductStockState,
            When(TakePaymentEvent)
                .TransitionTo(TakePaymentState));

        During(TakePaymentState,
            When(CreateOrderEvent)
                .TransitionTo(CreateOrderState));

        #endregion


        #region Fault-Companse State

        DuringAny(When(CreateOrderFaultEvent)
            .TransitionTo(CreateOrderFaultedState)
            .Then(context => context.Publish<Fault<TakePaymentEvent>>(new {context.Message})));


        DuringAny(When(TakePaymentEventFaultEvent)
            .TransitionTo(TakePaymentFaultedState)
            .Then(context => context.Publish<Fault<CheckProductStockEvent>>(new {context.Message})));

        DuringAny(When(CheckProductStockFaultEvent)
            .TransitionTo(CheckProductStockFaultedState)
            .Then(context => context.Publish<Fault<OrderProcessInitializationEvent>>(new {context.Message})));

        DuringAny(When(OrderProcessInitializationFaultEvent)
            .TransitionTo(OrderProcessInitializedFaultedState)
            .Then(context => context.Publish<OrderProcessFailedEvent>(new {OrderId = context.Saga.CorrelationId})));

        DuringAny(When(OrderProcessFailedEvent)
            .TransitionTo(OrderProcessFailedState));

        #endregion
    }

    #region Events

    public Event<OrderProcessInitializationEvent> OrderProcessInitializationEvent { get; }
    public Event<Fault<OrderProcessInitializationEvent>> OrderProcessInitializationFaultEvent { get; }

    public Event<CheckProductStockEvent> CheckProductStockEvent { get; }
    public Event<Fault<CheckProductStockEvent>> CheckProductStockFaultEvent { get; }

    public Event<TakePaymentEvent> TakePaymentEvent { get; }
    public Event<Fault<TakePaymentEvent>> TakePaymentEventFaultEvent { get; }

    public Event<CreateOrderEvent> CreateOrderEvent { get; }
    public Event<Fault<CreateOrderEvent>> CreateOrderFaultEvent { get; }

    public Event<OrderProcessFailedEvent> OrderProcessFailedEvent { get; }

    #endregion


    #region States

    public State OrderProcessInitializedState { get; }
    public State OrderProcessInitializedFaultedState { get; }

    public State CheckProductStockState { get; }
    public State CheckProductStockFaultedState { get; }

    public State TakePaymentState { get; }
    public State TakePaymentFaultedState { get; }

    public State CreateOrderState { get; }
    public State CreateOrderFaultedState { get; }

    public State OrderProcessFailedState { get; }

    #endregion
}
```