using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Reviews;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly SsmsDbContext _dbContext;

    public ReviewService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static ReviewResponse ToResponse(Review r) => new()
    {
        Id = r.Id,
        JobId = r.JobId,
        ReviewerId = r.ReviewerId,
        ReviewerName = r.Reviewer.FullName,
        RevieweeId = r.RevieweeId,
        RevieweeName = r.Reviewee.FullName,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt
    };

    public async Task<IReadOnlyList<ReviewResponse>> GetForJobAsync(Guid jobId)
    {
        var reviews = await _dbContext.Reviews
            .Include(r => r.Reviewer)
            .Include(r => r.Reviewee)
            .Where(r => r.JobId == jobId)
            .ToListAsync();
        return reviews.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetForUserAsync(Guid userId)
    {
        var reviews = await _dbContext.Reviews
            .Include(r => r.Reviewer)
            .Include(r => r.Reviewee)
            .Where(r => r.RevieweeId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return reviews.Select(ToResponse).ToList();
    }

    public async Task<ReviewResponse> CreateAsync(Guid reviewerUserId, CreateReviewRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");

        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == request.JobId)
            ?? throw new InvalidOperationException("Job not found.");

        if (job.Status != JobStatus.Closed)
            throw new InvalidOperationException("Reviews can only be left on Closed jobs.");

        // Confirm the reviewer is actually a real participant in this job —
        // either the Client or the AssignedWorker, nobody else.
        var reviewerIsClient = job.Client.UserId == reviewerUserId;
        var reviewerIsWorker = job.AssignedWorker?.UserId == reviewerUserId;
        if (!reviewerIsClient && !reviewerIsWorker)
            throw new UnauthorizedAccessException("You did not participate in this job.");

        // And confirm the Reviewee is the OTHER participant — you can't review yourself,
        // and you can't review someone unrelated to this job.
        var revieweeIsValid =
            (reviewerIsClient && job.AssignedWorker?.UserId == request.RevieweeId) ||
            (reviewerIsWorker && job.Client.UserId == request.RevieweeId);
        if (!revieweeIsValid)
            throw new InvalidOperationException("Invalid reviewee for this job.");

        var alreadyReviewed = await _dbContext.Reviews
            .AnyAsync(r => r.JobId == request.JobId && r.ReviewerId == reviewerUserId && r.RevieweeId == request.RevieweeId);
        if (alreadyReviewed)
            throw new InvalidOperationException("You have already reviewed this person for this job.");

        var review = new Review
        {
            JobId = request.JobId,
            ReviewerId = reviewerUserId,
            RevieweeId = request.RevieweeId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _dbContext.Reviews.Add(review);

        // If the Reviewee is a Worker, update their running RatingAverage.
        if (reviewerIsClient)
        {
            var workerProfile = job.AssignedWorker!;
            var allRatings = await _dbContext.Reviews
                .Where(r => r.RevieweeId == workerProfile.UserId)
                .Select(r => r.Rating)
                .ToListAsync();

            allRatings.Add(request.Rating); // include this new one before averaging
            workerProfile.RatingAverage = (decimal)allRatings.Average();
            workerProfile.CompletedJobsCount++;
        }

        await _dbContext.SaveChangesAsync();

        review.Reviewer = (await _dbContext.Users.FindAsync(reviewerUserId))!;
        review.Reviewee = (await _dbContext.Users.FindAsync(request.RevieweeId))!;
        return ToResponse(review);
    }
}