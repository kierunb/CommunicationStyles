using System.Runtime.CompilerServices;

namespace MassTransit.Contracts.InvoiceStateMachine;

public record FinalizeInvoice
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<FinalizeInvoice>(x => x.InvoiceId);
    }
}
