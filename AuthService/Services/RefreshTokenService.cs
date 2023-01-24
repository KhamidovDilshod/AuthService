using AuthService.Common.Authentication;
using AuthService.Domain;

namespace AuthService.Services;

public class RefreshTokenRepository : IRefreshTokenService
{
    public Task AddAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<JsonWebToken> CreateAccessTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAsync(string refreshToken, Guid userId)
    {
        throw new NotImplementedException();
    }
}