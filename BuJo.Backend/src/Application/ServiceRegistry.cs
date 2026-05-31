using BuJo.Application.Accounting;
using Microsoft.Extensions.DependencyInjection;

namespace BuJo.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserBotStateService, UserBotStateService>();

        return services;
    }
}