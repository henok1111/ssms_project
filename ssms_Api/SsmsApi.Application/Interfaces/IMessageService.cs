using SsmsApi.Application.DTOs.Messages;

namespace SsmsApi.Application.Interfaces;

public interface IMessageService
{
    Task<IReadOnlyList<MessageResponse>> GetForJobAsync(Guid jobId, Guid userId);

    Task<MessageResponse> SendAsync(Guid jobId, Guid senderId, SendMessageRequest request);

    Task<bool> MarkAsReadAsync(Guid jobId, Guid userId);
}