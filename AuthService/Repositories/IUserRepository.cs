using AuthService.Domain;

namespace AuthService.Repositories;

public interface IUserRepository
{
    Task<User> GetAsync(Guid id);
    Task<User> GetAsync(string email);
    Task AddAsync(User user);
    void UpdateAsync(User user);
}