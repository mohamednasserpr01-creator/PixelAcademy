using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Cache;
using PixelAcademy.Infrastructure.Data;
using PixelAcademy.Infrastructure.Identity;
using PixelAcademy.Infrastructure.Repositories;
using PixelAcademy.Infrastructure.Services;
using PixelAcademy.Infrastructure.UnitOfWork;

namespace PixelAcademy.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("PixelAcademy.Infrastructure")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "PixelAcademy_";
        });

        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISignedUrlService, SignedUrlService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
