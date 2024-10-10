using Crud.Application.Cache;
using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Infrastructure.Cache;
using Crud.Infrastructure.Data;
using Crud.Infrastructure.Repositories;
using Crud.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Crud.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration) =>
            services
                .AddLocalDbContext(configuration)
                .AddServices()
                .AddRedis();

    private static IServiceCollection AddLocalDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<QuoteChangeInterceptor>();
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<QuoteChangeInterceptor>();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(interceptor);
            //options.UseInMemoryDatabase("hr-quotes")
            //    .AddInterceptors(interceptor);
        });
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GeneralRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new InvalidOperationException("The Redis connection string is not set in the environment variables.");
            }
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        services.AddSingleton<IQuotesLengthCacheService, QuotesLengthCacheService>();
        services.AddSingleton<ICalculatedPairsCacheService, CalculatedPairsCacheService>();
        return services;
    }
}
