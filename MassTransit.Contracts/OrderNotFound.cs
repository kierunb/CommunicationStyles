namespace MassTransit.Contracts;

public record OrderNotFound
{
    public Guid OrderId { get; init; }
}
