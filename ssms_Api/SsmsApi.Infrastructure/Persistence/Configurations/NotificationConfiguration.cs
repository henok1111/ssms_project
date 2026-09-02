using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade); // no point keeping notifications for a deleted user

        builder.HasOne<Job>()
            .WithMany()
            .HasForeignKey(n => n.RelatedJobId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(n => n.Content).IsRequired();
    }
}