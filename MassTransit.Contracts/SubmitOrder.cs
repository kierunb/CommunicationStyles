using System.Runtime.CompilerServices;

namespace MassTransit.Contracts;

public record SubmitOrder
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<SubmitOrder>(x => x.OrderId);
    }
}
