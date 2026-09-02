using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        // Job -> ClientProfile (the poster). Many Jobs per Client, Restrict
        // so deleting a Client doesn't wipe out job history.
        builder.HasOne(j => j.Client)
            .WithMany()
            .HasForeignKey(j => j.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Job -> Category. Restrict — don't let deleting a Category
        // silently break existing jobs.
        builder.HasOne(j => j.Category)
            .WithMany()
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Job -> WorkerProfile (assigned worker). Nullable FK, so this worker
        // may not be set yet. Restrict for the same audit-trail reason.
        builder.HasOne(j => j.AssignedWorker)
            .WithMany()
            .HasForeignKey(j => j.AssignedWorkerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(j => j.Title).HasMaxLength(200).IsRequired();
        builder.Property(j => j.Budget).HasPrecision(12, 2);
        builder.Property(j => j.Location).HasMaxLength(200);
    }
}