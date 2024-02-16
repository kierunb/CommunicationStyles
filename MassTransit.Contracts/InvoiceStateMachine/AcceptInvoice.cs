using System.Runtime.CompilerServices;

namespace MassTransit.Contracts.InvoiceStateMachine;

public record AcceptInvoice
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }
    public required string UserAccepted { get; init; }
    public required DateTime AcceptedDate { get; init; }
    public string? Reason { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<AcceptInvoice>(x => x.InvoiceId);
    }
}


