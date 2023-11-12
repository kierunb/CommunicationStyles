namespace MassTransit.Contracts;

public record PingErrorMessage
{
    public Guid PingId { get; init; }
    public DateTime Timestamp { get; set; }
    public string Message { get; init; } = String.Empty;
}