namespace MassTransit.WebAPI.Models;

public record OrderModel
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; }
    public string Status { get; init; }
}
