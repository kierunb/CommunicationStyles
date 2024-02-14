using System.Runtime.CompilerServices;

namespace MassTransit.Contracts.InvoiceStateMachine;

public record SubmitInvoice
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }
    public required decimal Amount { get; init; }
    public required DateTime InvoiceDate { get; init; }
    public required string Status { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<SubmitInvoice>(x => x.InvoiceId);
    }
}


