namespace MassTransit.Contracts;

public record ProcessRaportCommand
{
    public Guid RaportId { get; init; }
    public string? RaportName { get; init; } = String.Empty;
    public string? RaportContent { get; init; } = String.Empty;
}
