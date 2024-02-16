namespace MassTransit.WebAPI.Models;

public class FinalizeInvoiceCommand
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }
}
