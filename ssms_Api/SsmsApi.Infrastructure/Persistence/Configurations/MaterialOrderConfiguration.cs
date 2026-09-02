using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class MaterialOrderConfiguration : IEntityTypeConfiguration<MaterialOrder>
{
    public void Configure(EntityTypeBuilder<MaterialOrder> builder)
    {
        builder.HasOne(mo => mo.JobMaterialRequest)
            .WithMany()
            .HasForeignKey(mo => mo.JobMaterialRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mo => mo.Supplier)
            .WithMany()
            .HasForeignKey(mo => mo.SupplierId)
            .OnDelete(DeleteBehavior.Restrict); // financial/order record — never silently cascade-delete

        builder.Property(mo => mo.TotalPrice).HasPrecision(12, 2);
    }
}