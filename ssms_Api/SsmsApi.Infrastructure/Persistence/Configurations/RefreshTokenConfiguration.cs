using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasOne(rt => rt.User)
            .WithMany() // ApplicationUser has no ICollection<RefreshToken> nav property — that's fine
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(rt => rt.Token).IsRequired();
    }
}