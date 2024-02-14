namespace MassTransit.WebAPI.Models;

public class InvoiceModel
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }
    public required decimal Amount { get; init; }
    public required DateTime InvoiceDate { get; init; }
    public required string Status { get; init; }
}
