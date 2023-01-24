using AuthService.Common.Authentication;
using AuthService.Domain;

namespace AuthService.Services;

public interface IRefreshTokenService
{
    Task AddAsync(Guid userId);
    Task<JsonWebToken> CreateAccessTokenAsync(string refreshToken);
    Task RevokeAsync(string refreshToken, Guid userId);
}