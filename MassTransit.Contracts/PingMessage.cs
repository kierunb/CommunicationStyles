namespace MassTransit.Contracts;

public record PingMessage
{
    public Guid PingId { get; init; }
    public DateTime Timestamp { get; set; }
    public string Message { get; init; }
}
