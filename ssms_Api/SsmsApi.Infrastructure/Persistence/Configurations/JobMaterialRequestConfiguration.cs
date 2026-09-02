using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class JobMaterialRequestConfiguration : IEntityTypeConfiguration<JobMaterialRequest>
{
    public void Configure(EntityTypeBuilder<JobMaterialRequest> builder)
    {
        builder.HasOne(jmr => jmr.Job)
            .WithMany()
            .HasForeignKey(jmr => jmr.JobId)
            .OnDelete(DeleteBehavior.Cascade); // request has no meaning without its Job

        builder.HasOne(jmr => jmr.MaterialItem)
            .WithMany()
            .HasForeignKey(jmr => jmr.MaterialItemId)
            .OnDelete(DeleteBehavior.Restrict); // protect historical requests if a product is removed

        builder.Property(jmr => jmr.UnitPriceAtRequest).HasPrecision(12, 2);
    }
}