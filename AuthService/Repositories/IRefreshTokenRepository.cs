using AuthService.Domain;

namespace AuthService.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetAsync(string token);
    Task AddAsync(RefreshToken token);
    void UpdateAsync(RefreshToken token);
}