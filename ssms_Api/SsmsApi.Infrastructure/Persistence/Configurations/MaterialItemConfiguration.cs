using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class MaterialItemConfiguration : IEntityTypeConfiguration<MaterialItem>
{
    public void Configure(EntityTypeBuilder<MaterialItem> builder)
    {
        builder.HasOne(m => m.Supplier)
            .WithMany(s => s.MaterialItems)
            .HasForeignKey(m => m.SupplierId)
            .OnDelete(DeleteBehavior.Cascade); // supplier deletes their own catalog with them

        builder.HasOne(m => m.Category)
            .WithMany()
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Unit).HasMaxLength(30).IsRequired();
        builder.Property(m => m.PricePerUnit).HasPrecision(12, 2);
    }
}