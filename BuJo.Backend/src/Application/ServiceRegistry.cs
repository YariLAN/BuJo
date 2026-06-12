using BuJo.Application.Accounting;
using BuJo.Application.Habits;
using Microsoft.Extensions.DependencyInjection;

namespace BuJo.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserBotStateService, UserBotStateService>();
        services.AddTransient<IHabitService, HabitService>();

        return services;
    }
}