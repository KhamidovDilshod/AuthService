namespace AuthService.Services;

public class ClaimsProvider : IClaimsProvider
{
    public async Task<Dictionary<string, string>> GetAsync(Guid userId)
    {
        return await Task.FromResult(new Dictionary<string, string>());
    }
}