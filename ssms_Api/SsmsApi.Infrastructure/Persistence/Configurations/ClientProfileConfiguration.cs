using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
{
    public void Configure(EntityTypeBuilder<ClientProfile> builder)
    {
        builder.HasIndex(c => c.UserId).IsUnique();

        builder.HasOne(c => c.User)
            .WithOne(u => u.ClientProfile)
            .HasForeignKey<ClientProfile>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}