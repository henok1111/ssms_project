using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class JobAttachmentConfiguration : IEntityTypeConfiguration<JobAttachment>
{
    public void Configure(EntityTypeBuilder<JobAttachment> builder)
    {
        builder.HasOne(ja => ja.Job)
            .WithMany()
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ja => ja.FileUrl).IsRequired();
    }
}