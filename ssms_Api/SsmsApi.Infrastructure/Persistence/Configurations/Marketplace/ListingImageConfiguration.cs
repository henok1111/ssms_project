using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities.Marketplace;

namespace SsmsApi.Infrastructure.Persistence.Configurations.Marketplace;

public class ListingMediaConfiguration : IEntityTypeConfiguration<ListingMedia>
{
    public void Configure(EntityTypeBuilder<ListingMedia> builder)
    {
        builder.HasOne(m => m.Listing)
            .WithMany(l => l.Media)
            .HasForeignKey(m => m.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.FileUrl).IsRequired();
    }
}