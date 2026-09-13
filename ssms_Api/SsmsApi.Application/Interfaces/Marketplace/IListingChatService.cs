using SsmsApi.Application.DTOs.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface IListingChatService
{
    // Gets the conversation between the current user and the seller for a
    // listing, creating it on first contact if it doesn't exist yet.
    Task<ListingConversationResponse> StartOrGetConversationAsync(Guid listingId, Guid buyerUserId);

    // All conversations a user is part of, either as buyer or as seller.
    Task<IReadOnlyList<ListingConversationResponse>> GetMyConversationsAsync(Guid userId);

    Task<IReadOnlyList<ListingMessageResponse>> GetMessagesAsync(Guid conversationId, Guid userId);

    Task<ListingMessageResponse> SendMessageAsync(Guid conversationId, Guid senderId, SendListingMessageRequest request);
}