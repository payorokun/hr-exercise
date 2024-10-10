using Microsoft.Extensions.DependencyInjection;

namespace Crud.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        if (context.IsInMemory) return;

        context.Database.EnsureCreated();
    }
}