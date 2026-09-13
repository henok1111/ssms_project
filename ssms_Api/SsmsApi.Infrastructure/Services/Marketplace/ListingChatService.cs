using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services.Marketplace;

public class ListingChatService : IListingChatService
{
    private readonly SsmsDbContext _dbContext;
    private readonly IRealtimeNotifier _realtime;

    public ListingChatService(SsmsDbContext dbContext, IRealtimeNotifier realtime)
    {
        _dbContext = dbContext;
        _realtime = realtime;
    }

    private static ListingConversationResponse ToConversationResponse(ListingConversation c) => new()
    {
        Id = c.Id,
        ListingId = c.ListingId,
        ListingTitle = c.Listing.Title,
        BuyerId = c.BuyerId,
        BuyerName = c.Buyer.FullName,
        SellerId = c.Listing.SellerId,
        SellerName = c.Listing.Seller.FullName,
        CreatedAt = c.CreatedAt
    };

    private static ListingMessageResponse ToMessageResponse(ListingMessage m) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        SenderName = m.Sender.FullName,
        Content = m.Content,
        IsRead = m.IsRead,
        CreatedAt = m.CreatedAt
    };

    public async Task<ListingConversationResponse> StartOrGetConversationAsync(Guid listingId, Guid buyerUserId)
    {
        var listing = await _dbContext.Listings
            .Include(l => l.Seller)
            .FirstOrDefaultAsync(l => l.Id == listingId)
            ?? throw new InvalidOperationException("Listing not found.");

        if (listing.SellerId == buyerUserId)
            throw new InvalidOperationException("You cannot start a conversation on your own listing.");

        var existing = await _dbContext.ListingConversations
            .Include(c => c.Buyer)
            .Include(c => c.Listing).ThenInclude(l => l.Seller)
            .FirstOrDefaultAsync(c => c.ListingId == listingId && c.BuyerId == buyerUserId);

        if (existing is not null)
            return ToConversationResponse(existing);

        var conversation = new ListingConversation
        {
            ListingId = listingId,
            BuyerId = buyerUserId
        };

        _dbContext.ListingConversations.Add(conversation);
        await _dbContext.SaveChangesAsync();

        conversation.Buyer = (await _dbContext.Users.FindAsync(buyerUserId))!;
        conversation.Listing = listing;
        return ToConversationResponse(conversation);
    }

    public async Task<IReadOnlyList<ListingConversationResponse>> GetMyConversationsAsync(Guid userId)
    {
        var conversations = await _dbContext.ListingConversations
            .Include(c => c.Buyer)
            .Include(c => c.Listing).ThenInclude(l => l.Seller)
            // A user sees a conversation if they're either the Buyer,
            // or the Seller of the Listing that conversation is about.
            .Where(c => c.BuyerId == userId || c.Listing.SellerId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return conversations.Select(ToConversationResponse).ToList();
    }

    public async Task<IReadOnlyList<ListingMessageResponse>> GetMessagesAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _dbContext.ListingConversations
            .Include(c => c.Listing)
            .FirstOrDefaultAsync(c => c.Id == conversationId)
            ?? throw new InvalidOperationException("Conversation not found.");

        var isParticipant = conversation.BuyerId == userId || conversation.Listing.SellerId == userId;
        if (!isParticipant)
            throw new UnauthorizedAccessException("You are not part of this conversation.");

        var messages = await _dbContext.ListingMessages
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(ToMessageResponse).ToList();
    }

    public async Task<ListingMessageResponse> SendMessageAsync(
        Guid conversationId, Guid senderId, SendListingMessageRequest request)
    {
        var conversation = await _dbContext.ListingConversations
            .Include(c => c.Listing)
            .FirstOrDefaultAsync(c => c.Id == conversationId)
            ?? throw new InvalidOperationException("Conversation not found.");

        var isParticipant = conversation.BuyerId == senderId || conversation.Listing.SellerId == senderId;
        if (!isParticipant)
            throw new UnauthorizedAccessException("You are not part of this conversation.");

        var message = new ListingMessage
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = request.Content,
            IsRead = false
        };

        _dbContext.ListingMessages.Add(message);
        await _dbContext.SaveChangesAsync();

        message.Sender = (await _dbContext.Users.FindAsync(senderId))!;
        var response = ToMessageResponse(message);

        // Reuse the SAME SignalR group-push mechanism as Job messages —
        // just a different group name so the two chat systems don't collide.
       await _realtime.SendToGroupAsync($"listing-conversation-{conversationId}", "ReceiveListingMessage", response);
        return response;
    }
}