using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Application.Tasks;
using BuJo.Application.Tasks.Abstractions;
using BuJo.Application.Habits;
using Microsoft.Extensions.DependencyInjection;

namespace BuJo.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserBotStateService, UserBotStateService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IHabitService, HabitService>();

        return services;
    }
}