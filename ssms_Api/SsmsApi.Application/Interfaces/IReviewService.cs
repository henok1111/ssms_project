using SsmsApi.Application.DTOs.Reviews;

namespace SsmsApi.Application.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewResponse>> GetForJobAsync(Guid jobId);

    Task<IReadOnlyList<ReviewResponse>> GetForUserAsync(Guid userId); // reviews RECEIVED by this user

    Task<ReviewResponse> CreateAsync(Guid reviewerUserId, CreateReviewRequest request);
}