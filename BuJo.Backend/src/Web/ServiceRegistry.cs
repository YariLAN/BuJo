using Microsoft.Extensions.DependencyInjection;

namespace BuJo.Web;

public static class ServiceRegistry
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        return services;
    }
}