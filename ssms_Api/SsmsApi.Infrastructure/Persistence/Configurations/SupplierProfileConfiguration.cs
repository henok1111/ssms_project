using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class SupplierProfileConfiguration : IEntityTypeConfiguration<SupplierProfile>
{
    public void Configure(EntityTypeBuilder<SupplierProfile> builder)
    {
        builder.HasIndex(s => s.UserId).IsUnique();

        builder.HasOne(s => s.User)
            .WithOne(u => u.SupplierProfile)
            .HasForeignKey<SupplierProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.ShopName).HasMaxLength(150).IsRequired();
    }
}