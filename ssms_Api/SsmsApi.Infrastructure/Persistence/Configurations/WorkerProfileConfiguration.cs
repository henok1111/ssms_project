using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class WorkerProfileConfiguration : IEntityTypeConfiguration<WorkerProfile>
{
    public void Configure(EntityTypeBuilder<WorkerProfile> builder)
    {
        builder.HasIndex(w => w.UserId).IsUnique(); // enforces the 1-to-1

        builder.HasOne(w => w.User)
            .WithOne(u => u.WorkerProfile)
            .HasForeignKey<WorkerProfile>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(w => w.ServiceArea).HasMaxLength(200);
        builder.Property(w => w.RatingAverage).HasPrecision(3, 2); // e.g. 4.75
    }
}