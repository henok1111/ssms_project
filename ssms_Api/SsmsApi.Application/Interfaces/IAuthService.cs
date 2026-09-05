using SsmsApi.Application.DTOs.Auth;

namespace SsmsApi.Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string[] Errors, AuthResponse? Response, string? AccessToken, string? RefreshToken)>
        RegisterAsync(RegisterRequest request);

    Task<(bool Success, string[] Errors, AuthResponse? Response, string? AccessToken, string? RefreshToken)>
        LoginAsync(LoginRequest request);
Task<(bool Success, string[] Errors, string? AccessToken, string? RefreshToken)>
    RefreshTokenAsync(string refreshToken);

    
    Task<(bool Success, string[] Errors)> ConfirmEmailAsync(Guid userId, string token);

    Task<(bool Success, string[] Errors)> ForgotPasswordAsync(string email);

    Task<(bool Success, string[] Errors)> ResetPasswordAsync(string email, string token, string newPassword);


}