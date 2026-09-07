using SsmsApi.Application.DTOs.Attachments;

namespace SsmsApi.Application.Interfaces;

public interface IJobAttachmentService
{
    Task<IReadOnlyList<JobAttachmentResponse>> GetForJobAsync(Guid jobId);

    Task<JobAttachmentResponse> UploadAsync(Guid jobId, Guid userId, Stream fileStream, string fileName, string contentType);

    Task<bool> DeleteAsync(Guid attachmentId, Guid userId);
}