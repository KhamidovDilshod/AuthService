namespace AuthService.Services;

public interface IClaimsProvider
{
    Task<Dictionary<string, string>> GetAsync(Guid userId);
}