using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Crud.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services) =>
            services
                .AddApplicationMediator()
                .AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly)
                .AddMapping();

    private static IServiceCollection AddApplicationMediator(
        this IServiceCollection services) =>
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

    private static IServiceCollection AddMapping(this IServiceCollection services) =>
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
}
