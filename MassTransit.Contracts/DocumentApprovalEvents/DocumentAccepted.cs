using MassTransit.Contracts.InvoiceStateMachine;
using System.Runtime.CompilerServices;

namespace MassTransit.Contracts.DocumentApprovalEvents;

public record DocumentAccepted
{
    public Guid DocumentId { get; init; }
    public string AcceptedBy { get; init; } = default!;
    public DateTime AcceptedAt { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<DocumentAccepted>(x => x.DocumentId);
    }
}
