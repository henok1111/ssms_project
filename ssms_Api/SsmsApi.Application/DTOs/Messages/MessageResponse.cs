namespace SsmsApi.Application.DTOs.Messages;

public class MessageResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}