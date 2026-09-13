using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities.Marketplace;

public class ListingMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public ListingConversation Conversation { get; set; } = null!;

    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}