namespace MassTransit.Contracts;
public record RaportProcessedEvent
{
    public Guid RaportId { get; init; }
    public string? RaportStatus { get; init; } = String.Empty;
}