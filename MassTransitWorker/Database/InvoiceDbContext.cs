using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Worker.Sagas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.Worker.Database;

public class InvoiceDbContext : SagaDbContext
{

    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations { get { yield return new InvoiceStateMap(); } }
}


public class InvoiceStateMap : SagaClassMap<InvoiceState>
{
    protected override void Configure(EntityTypeBuilder<InvoiceState> entity, ModelBuilder model)
    {
        base.Configure(entity, model);

        entity.Property(x => x.CurrentState).HasMaxLength(40);

        entity.Property(x => x.InvoiceNumber).HasMaxLength(40);
    }
}
