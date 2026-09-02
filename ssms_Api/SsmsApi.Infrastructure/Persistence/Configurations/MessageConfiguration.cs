using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(m => m.Job)
            .WithMany()
            .HasForeignKey(m => m.JobId)
            .OnDelete(DeleteBehavior.Cascade); // chat has no meaning outside its Job

        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.Content).IsRequired();
    }
}