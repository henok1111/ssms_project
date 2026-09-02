using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasOne(q => q.Job)
            .WithMany()
            .HasForeignKey(q => q.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Job should only ever have one Quote — enforce it here.
        builder.HasIndex(q => q.JobId).IsUnique();

        builder.Property(q => q.LaborCost).HasPrecision(12, 2);
        builder.Property(q => q.MaterialsCost).HasPrecision(12, 2);

        // TotalCost is a C#-only computed property (=> LaborCost + MaterialsCost),
        // not a real column. Without this line, EF Core will try to map it
        // and crash at startup looking for a "TotalCost" column that doesn't exist.
        builder.Ignore(q => q.TotalCost);
    }
}