using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AuthService.Common.Authentication;
using AuthService.Common.Context;
using AuthService.Common.Handlers;
using AuthService.Common.RabbitMq;
using AuthService.Domain;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Extension;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<JwtOptions>("jwt");
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

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("jwt");
        services.Configure<JwtOptions>(jwtSection);
        var rabbitMqSection = configuration.GetSection("rabbitMq");
        services.Configure<RabbitMqOptions>(rabbitMqSection);

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordHasher<RefreshToken>, PasswordHasher<RefreshToken>>();
        services.AddScoped<IClaimsProvider, ClaimsProvider>();
        services.AddScoped<IBusPublisher, BusPublisher>();
        services.AddScoped<IHandler, Handler>();

        services.AddHostedService<BusSubscriber>();

        // services.AddScoped(typeof(IEventHandler<>));
        // services.AddScoped(typeof(ICommandHandler<>));
        // services.AddScoped<IBusSubscriber, BusSubscriber>();

        // var builder = new ContainerBuilder();
        // builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly()!).AsImplementedInterfaces();
        // // builder.RegisterType<PasswordHasher<User>>().As<IPasswordHasher<User>>();
        // // builder.AddRabbitMq();
        // return new AutofacServiceProvider(builder.Build());
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

    public static T Bind<T>(this T model, Expression<Func<T, object>> expression, object value)
        => model.Bind<T, object>(expression, value);

    public static T BindId<T>(this T model, Expression<Func<T, Guid>> expression)
        => model.Bind<T, Guid>(expression, Guid.NewGuid());

    private static TModel Bind<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression,
        object value)
    {
        var memberExpression = expression.Body as MemberExpression;
        if (memberExpression == null)
        {
            memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
        }

        var propertyName = memberExpression.Member.Name.ToLowerInvariant();
        var modelType = model.GetType();
        var field = modelType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith($"<{propertyName}>"));
        if (field == null)
        {
            return model;
        }

        field.SetValue(model, value);

        return model;
    }

    public static string Underscore(this string value)
        => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
}