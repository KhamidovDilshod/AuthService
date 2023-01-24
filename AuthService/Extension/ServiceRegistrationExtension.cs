using System.Text;
using AuthService.Common.Authentication;
using AuthService.Common.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Extension;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("jwt");
        var options = configuration.GetOptions<JwtOptions>("jwt");

        services.Configure<JwtOptions>(section);
        services.AddSingleton(options);
        services.AddSingleton<IJwtHandler, JwtHandler>();

        services.AddAuthentication().AddJwtBearer(cfg =>
        {
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                ValidIssuer = options.Issuer,
                ValidAudience = options.ValidAudience,
                ValidateAudience = options.ValidateAudience,
                ValidateLifetime = options.ValidateLifetime,
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }

    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetValue<string>("Postgres:ConnectionString") ??
                                  throw new Exception($"Cant find connection string at: '{DateTime.UtcNow}'");
        services.AddDbContext<AuthContext>(options =>
            options.UseNpgsql(connectionString));
        return services;
    }

    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(section).Bind(model);

        return model;
    }

    public static long ToTimeStamp(this DateTime dateTime)
    {
        var centuryBegin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedDate = dateTime.Subtract(new TimeSpan(centuryBegin.Ticks));

        return expectedDate.Ticks / 1000;
    }

    public static string Underscore(this string value)
        => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
}