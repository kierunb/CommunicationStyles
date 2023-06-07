using System.Runtime.CompilerServices;

namespace MassTransit.Contracts;

public record GetOrder
{
    public Guid OrderId { get; init; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<GetOrder>(x => x.OrderId);
    }
}
