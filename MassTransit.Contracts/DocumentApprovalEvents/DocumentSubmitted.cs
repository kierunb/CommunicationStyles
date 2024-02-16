using System.Runtime.CompilerServices;

namespace MassTransit.Contracts.DocumentApprovalEvents;

public record DocumentSubmitted
{
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = default!;
    public string DocumentType { get; init; } = default!;
    public string DocumentContent { get; init; } = default!;

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<DocumentSubmitted>(x => x.DocumentId);
    }
}
