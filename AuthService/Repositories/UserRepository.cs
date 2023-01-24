using AuthService.Common.Postgres;
using AuthService.Domain;

namespace AuthService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IPostgresRepository<User> _repository;

    public UserRepository(IPostgresRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<User> GetAsync(Guid id)
        => await _repository.GetAsync(id);

    public async Task<User> GetAsync(string email)
        => await _repository.GetAsync(x => x.Email == email.ToLowerInvariant());

    public Task AddAsync(User user)
        => _repository.AddAsync(user);

    public void UpdateAsync(User user)
        => _repository.Update(user);
}