using Crud.Application.Cache;
using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Infrastructure.Cache;
using Crud.Infrastructure.Data;
using Crud.Infrastructure.Repositories;
using Crud.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Crud.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services) =>
            services
                .AddLocalDbContext()
                .AddServices()
                .AddRedis();

    private static IServiceCollection AddLocalDbContext(this IServiceCollection services)
    {
        services.AddSingleton<QuoteChangeInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<QuoteChangeInterceptor>();
            options.UseInMemoryDatabase("hr-quotes")
                .AddInterceptors(interceptor);
        });
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GeneralRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IQuotesLengthCacheService, QuotesLengthCacheService>();
        services.AddScoped<ICalculatedPairsCacheService, CalculatedPairsCacheService>();
        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse("localhost:6379");
            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddScoped<IQuotesLengthCacheService, QuotesLengthCacheService>();
        return services;
    }
}
