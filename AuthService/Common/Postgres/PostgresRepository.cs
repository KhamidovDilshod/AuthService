using System.Linq.Expressions;
using AuthService.Common.Context;
using AuthService.Common.Types;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Common.Postgres;

public class PostgresRepository<TEntity> : IPostgresRepository<TEntity>
    where TEntity : class, IIdentifiable, new()
{
    private readonly AuthContext _authContext;

    public PostgresRepository(AuthContext authContext)
    {
        _authContext = authContext;
    }

    public async Task<TEntity> GetAsync(Guid id)
        => await GetAsync(e => e.Id == id);

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        => await _authContext.Set<TEntity>().FirstOrDefaultAsync(predicate) ?? new TEntity();

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        => await _authContext.Set<TEntity>().Where(predicate).ToListAsync();

    public async Task AddAsync(TEntity entity)
        => await _authContext.Set<TEntity>().AddAsync(entity);

    public void Update(TEntity entity)
        => _authContext.Set<TEntity>().Update(entity);

    public async Task DeleteAsync(Guid id)
        => _authContext.Set<TEntity>().Remove(await GetAsync(id));

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        => _authContext.Set<TEntity>().AnyAsync(predicate);
}