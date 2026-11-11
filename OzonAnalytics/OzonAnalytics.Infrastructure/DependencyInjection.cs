using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OzonAnalytics.Application.Interfaces.Analytic;
using OzonAnalytics.Application.Interfaces.Auth;
using OzonAnalytics.Application.Interfaces.Orders;
using OzonAnalytics.Application.Interfaces.Ozon;
using OzonAnalytics.Application.Interfaces.Repositories;
using OzonAnalytics.Application.Interfaces.Security;
using OzonAnalytics.Application.Interfaces.Shops;
using OzonAnalytics.Application.Interfaces.Transaction;
using OzonAnalytics.Application.Services.AnalyticService;
using OzonAnalytics.Application.Services.ShopService;
using OzonAnalytics.Application.Services.Sync;
using OzonAnalytics.Application.Services.UserService;
using OzonAnalytics.Domain.Entities;
using OzonAnalytics.Infrastructure.Auth;
using OzonAnalytics.Infrastructure.Database;
using OzonAnalytics.Infrastructure.Ozon;
using OzonAnalytics.Infrastructure.Ozon.Fake;
using OzonAnalytics.Infrastructure.Repositories.AnalyticsRepository;
using OzonAnalytics.Infrastructure.Repositories.OrderItemRepository;
using OzonAnalytics.Infrastructure.Repositories.OrderRepositrory;
using OzonAnalytics.Infrastructure.Repositories.ProductRepository;
using OzonAnalytics.Infrastructure.Repositories.ShopRepository;
using OzonAnalytics.Infrastructure.Repositories.SyncLogRepository;
using OzonAnalytics.Infrastructure.Repositories.UserRepository;
using OzonAnalytics.Infrastructure.Services.EncryptedService;
using OzonAnalytics.Infrastructure.Transaction;
using System.Text;

namespace OzonAnalytics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddIdentity(services);
        AddAuthentication(services, configuration);
        AddRepositories(services, configuration);

        return services;
    }

    private static void AddDatabase(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    private static void AddIdentity(
        IServiceCollection services)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    private static void AddAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection("JwtOptions"));

        var jwtOptions = configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration was not found.");

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is not configured.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtOptions.SecretKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();
    }

    private static void AddRepositories(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var FakeOzon = configuration.GetValue<bool>("Ozon:UseFake");

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ISyncLogRepository, SyncLogRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();


        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<ISyncOrchestrationService, SyncOrchestrationService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddHttpContextAccessor();
        services.AddScoped<IFindCurrentUser, FindCurrentUser>();
        if (FakeOzon)
        {
            services.AddScoped<IOzonClient, FakeOzonClient>();
        }
        else
        {
            services.AddHttpClient<IOzonClient, OzonClient>();
        }
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddDataProtection();
    }
}