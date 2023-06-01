namespace MassTransit.Contracts;

public record GettingStartedMessage
{
    public int Number { get; set; }
    public string Payload { get; set; } = string.Empty;
}