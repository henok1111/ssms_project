using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.HasOne(d => d.Job)
            .WithMany()
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Restrict); // accountability record

        builder.HasOne(d => d.RaisedBy)
            .WithMany()
            .HasForeignKey(d => d.RaisedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.Reason).IsRequired();
    }
}