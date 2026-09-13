using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities.Marketplace;

namespace SsmsApi.Infrastructure.Persistence.Configurations.Marketplace;

public class ListingConversationConfiguration : IEntityTypeConfiguration<ListingConversation>
{
    public void Configure(EntityTypeBuilder<ListingConversation> builder)
    {
        builder.HasOne(c => c.Listing)
            .WithMany()
            .HasForeignKey(c => c.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Buyer)
            .WithMany()
            .HasForeignKey(c => c.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        // One conversation per (Listing, Buyer) pair — matches our design:
        // a buyer only ever has ONE thread with the seller per listing.
        builder.HasIndex(c => new { c.ListingId, c.BuyerId }).IsUnique();
    }
}