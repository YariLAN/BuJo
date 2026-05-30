using BuJo.Application.Accounting;
using BuJo.Data.Repositories.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuJo.Data;

public static class ServiceRegistry
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddContext(configuration)
            .AddRepositories();
    }
    
    private static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ConnectionOptions>().BindConfiguration(ConnectionOptions.SectionName);
        
        var options = configuration
            .GetRequiredSection(ConnectionOptions.SectionName)
            .Get<ConnectionOptions>() 
                      ?? throw new ArgumentNullException();
        
        services.AddDbContext<DataContext>(builder =>
        {
            builder.UseNpgsql(options.ConnectionString, optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure();
            });
            
            builder.UseSnakeCaseNamingConvention();
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        
        return services;
    }
}