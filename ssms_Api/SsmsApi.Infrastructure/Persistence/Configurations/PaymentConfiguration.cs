using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(p => p.Job)
            .WithMany()
            .HasForeignKey(p => p.JobId)
            .OnDelete(DeleteBehavior.Restrict); // financial audit trail — never cascade-delete

        builder.HasOne(p => p.Quote)
            .WithMany()
            .HasForeignKey(p => p.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.JobId).IsUnique(); // one Payment per Job
        builder.HasIndex(p => p.TxRef).IsUnique();  // Chapa tx_ref must be unique

        builder.Property(p => p.Amount).HasPrecision(12, 2);
        builder.Property(p => p.PlatformCommission).HasPrecision(12, 2);
        builder.Property(p => p.TxRef).HasMaxLength(100).IsRequired();
        builder.Property(p => p.GatewayProvider).HasMaxLength(50);

        builder.Ignore(p => p.AmountReleasedToWorker); // computed property, not a column
    }
}