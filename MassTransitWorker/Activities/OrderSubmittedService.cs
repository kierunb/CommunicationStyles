using MassTransit.Contracts;

namespace MassTransit.Worker.Activities;

public interface IOrderSubmittedService
{
    Task OnOrderSubmitted(Guid orderId);
}

public class OrderSubmittedService : IOrderSubmittedService
{
    IPublishEndpoint _publishEndpoint;

    public OrderSubmittedService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task OnOrderSubmitted(Guid orderId)
    {
        await _publishEndpoint.Publish<OrderSubmitted>(new { OrderId = orderId });
    }
}
