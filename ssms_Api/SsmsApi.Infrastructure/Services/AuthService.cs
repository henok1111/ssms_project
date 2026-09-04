using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SsmsApi.Application.DTOs.Auth;

using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SsmsDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SsmsDbContext dbContext,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<(bool, string[], AuthResponse?, string?, string?)> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            return (false, new[] { "Email is already registered." }, null, null, null);

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return (false, createResult.Errors.Select(e => e.Description).ToArray(), null, null, null);

        await _userManager.AddToRoleAsync(user, request.Role.ToString());

        // Atomically create the matching profile based on Role.
        switch (request.Role)
        {
            case UserRole.Worker:
                _dbContext.WorkerProfiles.Add(new WorkerProfile
                {
                    UserId = user.Id,
                    WorkerType = request.WorkerType ?? Domain.Enums.WorkerType.OnSite,
                    ServiceArea = request.ServiceArea,
                    ApprovalStatus = ApprovalStatus.Pending
                });
                break;

            case UserRole.Client:
                _dbContext.ClientProfiles.Add(new ClientProfile { UserId = user.Id });
                break;

            case UserRole.Supplier:
                _dbContext.SupplierProfiles.Add(new SupplierProfile
                {
                    UserId = user.Id,
                    ShopName = request.ShopName ?? string.Empty,
                    Location = request.SupplierLocation ?? string.Empty,
                    ApprovalStatus = ApprovalStatus.Pending
                });
                break;
        }

        await _dbContext.SaveChangesAsync();

        // Send email confirmation link
        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedConfirmToken = Uri.EscapeDataString(confirmToken);
        var confirmLink = $"{_configuration["ClientAppUrl"]}/confirm-email?userId={user.Id}&token={encodedConfirmToken}";

        await _emailService.SendEmailAsync(
            user.Email!,
            "Confirm your SSMS account",
            $"<p>Hi {user.FullName},</p><p>Please confirm your account by clicking below:</p>" +
            $"<p><a href='{confirmLink}'>Confirm Email</a></p>"
        );

        var accessToken = _tokenService.GenerateAccessToken(user, request.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await _dbContext.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = request.Role.ToString()
        };

        return (true, Array.Empty<string>(), response, accessToken, refreshToken);
    }

    public async Task<(bool, string[], AuthResponse?, string?, string?)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return (false, new[] { "Invalid email or password." }, null, null, null);

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            return (false, new[] { "Invalid email or password." }, null, null, null);
        }

        if (await _userManager.IsLockedOutAsync(user))
            return (false, new[] { "Account is locked. Try again later." }, null, null, null);

        await _userManager.ResetAccessFailedCountAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await _dbContext.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = user.Role.ToString()
        };

        return (true, Array.Empty<string>(), response, accessToken, refreshToken);
    }

    public async Task<(bool, string[])> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return (false, new[] { "User not found." });

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded
            ? (true, Array.Empty<string>())
            : (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool, string[])> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return (true, Array.Empty<string>()); // never reveal whether the email exists

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(resetToken);
        var resetLink = $"{_configuration["ClientAppUrl"]}/reset-password?email={email}&token={encodedToken}";

        await _emailService.SendEmailAsync(
            email,
            "Reset your SSMS password",
            $"<p>Click below to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>"
        );

        return (true, Array.Empty<string>());
    }

    public async Task<(bool, string[])> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, new[] { "Invalid request." });

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded
            ? (true, Array.Empty<string>())
            : (false, result.Errors.Select(e => e.Description).ToArray());
    }
}