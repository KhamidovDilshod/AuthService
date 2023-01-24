namespace AuthService.Common.Postgres;

public static class Extensions
{
    public static IServiceCollection AddPostgresRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPostgresRepository<>), typeof(PostgresRepository<>));
        return services;
    }
}