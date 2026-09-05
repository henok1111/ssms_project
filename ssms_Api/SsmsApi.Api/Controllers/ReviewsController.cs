using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Reviews;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var reviews = await _reviewService.GetForJobAsync(jobId);
        return Ok(reviews);
    }

    [HttpGet("user/{userId:guid}")]
    [AllowAnonymous] // public profile ratings should be visible to anyone browsing
    public async Task<IActionResult> GetForUser(Guid userId)
    {
        var reviews = await _reviewService.GetForUserAsync(userId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        var review = await _reviewService.CreateAsync(CurrentUserId, request);
        return Ok(review);
    }
}