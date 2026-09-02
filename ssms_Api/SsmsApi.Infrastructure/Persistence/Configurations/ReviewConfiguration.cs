using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne(r => r.Job)
            .WithMany()
            .HasForeignKey(r => r.JobId)
            .OnDelete(DeleteBehavior.Restrict); // preserve review history even if Job record changes

        // Both of these point to ApplicationUser — EF needs each one
        // explicitly separated with its OWN HasForeignKey, or it can't
        // tell "Reviewer" and "Reviewee" apart.
        builder.HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Reviewee)
            .WithMany()
            .HasForeignKey(r => r.RevieweeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.Rating).IsRequired();

        // One reviewer can only review the same person once per job.
        builder.HasIndex(r => new { r.JobId, r.ReviewerId, r.RevieweeId }).IsUnique();
    }
}