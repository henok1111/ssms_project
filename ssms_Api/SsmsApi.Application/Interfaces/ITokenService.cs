using SsmsApi.Domain.Entities;

namespace SsmsApi.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, string role);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiry();
}