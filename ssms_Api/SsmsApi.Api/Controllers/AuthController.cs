using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Auth;

using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        Response.Cookies.Append("access_token", accessToken, accessCookieOptions);

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
    }
}