using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities.Marketplace;

namespace SsmsApi.Infrastructure.Persistence.Configurations.Marketplace;

public class ListingOfferConfiguration : IEntityTypeConfiguration<ListingOffer>
{
    public void Configure(EntityTypeBuilder<ListingOffer> builder)
    {
        builder.HasOne(o => o.Listing)
            .WithMany(l => l.Offers)
            .HasForeignKey(o => o.ListingId)
            .OnDelete(DeleteBehavior.Cascade); // offers have no meaning without their listing

        builder.HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(o => o.OfferedPrice).HasPrecision(12, 2);

        // A buyer shouldn't spam the same listing with unlimited pending offers.
        builder.HasIndex(o => new { o.ListingId, o.BuyerId, o.Status });
    }
}