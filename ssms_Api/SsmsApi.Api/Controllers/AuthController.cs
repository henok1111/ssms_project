using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SsmsApi.Domain.Entities;
using SsmsApi.Application.Interfaces;
using SsmsApi.Infrastructure.Persistence;
namespace SsmsApi.Api.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;
private readonly IFileStorageService _fileStorageService;
private readonly SsmsDbContext _db;

public AuthController(
    IAuthService authService,
    IFileStorageService fileStorageService,
    SsmsDbContext db)
{
    _authService = authService;
    _fileStorageService = fileStorageService;
    _db = db;
}
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, errors, response, accessToken, refreshToken) = await _authService.RegisterAsync(request);
        if (!success)
            return BadRequest(new { errors });

        SetAuthCookies(accessToken!, refreshToken!);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, errors, response, accessToken, refreshToken) = await _authService.LoginAsync(request);
        if (!success)
            return Unauthorized(new { errors });

        SetAuthCookies(accessToken!, refreshToken!);
        return Ok(response);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var (success, errors) = await _authService.ConfirmEmailAsync(userId, token);
        return success ? Ok(new { message = "Email confirmed successfully." }) : BadRequest(new { errors });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var (success, errors) = await _authService.ForgotPasswordAsync(email);
        return success ? Ok(new { message = "If that email exists, a reset link has been sent." }) : BadRequest(new { errors });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var (success, errors) = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        return success ? Ok(new { message = "Password reset successfully." }) : BadRequest(new { errors });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return Ok(new { message = "Logged out." });
    }

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        Response.Cookies.Append("access_token", accessToken, accessCookieOptions);

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
    }

[HttpPost("refresh")]
public async Task<IActionResult> Refresh()
{
    var refreshToken = Request.Cookies["refresh_token"];
    if (string.IsNullOrEmpty(refreshToken))
        return Unauthorized(new { message = "No refresh token provided." });

    var (success, errors, accessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);
    if (!success)
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return Unauthorized(new { errors });
    }

    SetAuthCookies(accessToken!, newRefreshToken!);
    return Ok(new { message = "Token refreshed." });
}

[Authorize]
[HttpPost("profile-picture")]
public async Task<IActionResult> UploadProfilePicture(IFormFile file)
{
    if (file is null || file.Length == 0)
        return BadRequest(new { message = "No file was uploaded." });

    if (file.Length > 5 * 1024 * 1024)
        return BadRequest(new { message = "Profile picture must not exceed 5 MB." });

    var allowedTypes = new[]
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    if (!allowedTypes.Contains(file.ContentType.ToLower()))
        return BadRequest(new { message = "Only JPG, PNG, and WEBP images are allowed." });

    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (!Guid.TryParse(userIdClaim, out var userId))
        return Unauthorized();

    await using var stream = file.OpenReadStream();

    var (success, profilePictureUrl) =
        await _authService.UploadProfilePictureAsync(
            userId,
            stream,
            file.FileName,
            file.ContentType);

    if (!success)
        return BadRequest(new { message = "Failed to upload profile picture." });

    return Ok(new
    {
        profilePictureUrl
    });
}
[Authorize]
[HttpGet("me")]
public async Task<IActionResult> GetMe()
{
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (!Guid.TryParse(userIdClaim, out var userId))
        return Unauthorized();

    var response = await _authService.GetCurrentUserAsync(userId);

    if (response is null)
        return NotFound(new { message = "User not found." });

    return Ok(response);
}

}

