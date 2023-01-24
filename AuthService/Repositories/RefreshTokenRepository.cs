using AuthService.Common.Postgres;
using AuthService.Domain;

namespace AuthService.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IPostgresRepository<RefreshToken> _repository;

    public RefreshTokenRepository(IPostgresRepository<RefreshToken> repository)
    {
        _repository = repository;
    }

    public async Task<RefreshToken> GetAsync(string token)
        => await _repository.GetAsync(x => x.Token == token);

    public async Task AddAsync(RefreshToken token)
        => await _repository.AddAsync(token);

    public void UpdateAsync(RefreshToken token)
        =>  _repository.Update(token);
}