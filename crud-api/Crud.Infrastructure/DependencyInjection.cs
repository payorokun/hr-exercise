using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Infrastructure.Data;
using Crud.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crud.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services) =>
            services
                .AddLocalDbContext()
                .AddServices();

    private static IServiceCollection AddLocalDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("hr-quotes"));
        return services;
    }
        

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GeneralRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        return services;
    }
        
    
}
