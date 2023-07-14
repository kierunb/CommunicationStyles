using MassTransit.Worker.Sagas;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Worker.Database;

public class OrderStateMap :
        SagaClassMap<OrderState>
{
    protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
    {
        base.Configure(entity, model);

        entity.Property(x => x.CurrentState).HasMaxLength(40);

        entity.Property(x => x.OrderNumber).HasMaxLength(40);
    }
}
