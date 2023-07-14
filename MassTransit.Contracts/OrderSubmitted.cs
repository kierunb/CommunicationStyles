namespace MassTransit.Contracts;

public record OrderSubmitted
{
    Guid OrderId { get; }
}
