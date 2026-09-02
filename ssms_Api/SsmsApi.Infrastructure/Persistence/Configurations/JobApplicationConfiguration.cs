using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasOne(ja => ja.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ja => ja.Worker)
            .WithMany()
            .HasForeignKey(ja => ja.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ja => ja.ProposedPrice).HasPrecision(12, 2);

        // A worker shouldn't be able to apply to the same job twice.
        builder.HasIndex(ja => new { ja.JobId, ja.WorkerId }).IsUnique();
    }
}