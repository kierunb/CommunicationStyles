using System.Runtime.CompilerServices;

namespace MassTransit.Contracts.InvoiceStateMachine;

public record GetInvoice
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<GetInvoice>(x => x.InvoiceId);
    }
}
