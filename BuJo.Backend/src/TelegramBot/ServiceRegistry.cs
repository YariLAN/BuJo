using BuJo.TelegramBot.Handlers;
using BuJo.TelegramBot.Handlers.Callbacks;
using BuJo.TelegramBot.Handlers.Commands;
using BuJo.TelegramBot.Handlers.Messages;
using BuJo.TelegramBot.Services;
using BuJo.TelegramBot.Services.Main;
using BuJo.TelegramBot.Services.Settings;
using BuJo.TelegramBot.Services.Tasks;
using BuJo.TelegramBot.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BuJo.TelegramBot;

public static class ServiceRegistry
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TelegramOptions>().BindConfiguration(TelegramOptions.SectionName);

        var options = configuration
                          .GetRequiredSection(TelegramOptions.SectionName)
                          .Get<TelegramOptions>()
                      ?? throw new ArgumentNullException();

        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(options.TokenRequired));

        services.AddHostedService<TelegramPollingWorker>();

        services.AddScoped<UpdateDispatcher>();

        services.AddScoped<MenuRenderer>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ISettingsMenuService, SettingsMenuService>();
        services.AddScoped<ITasksMenuService, TasksMenuService>();

        services.AddScoped<ICommandHandler, StartCommandHandler>();
        services.AddScoped<ICommandHandler, TasksCommandHandler>();
        services.AddScoped<ICommandHandler, CreateTaskCommandHandler>();
        services.AddScoped<ICallbackHandler, MenuCallbackHandler>();
        services.AddScoped<ICallbackHandler, SettingCallbackHandler>();
        services.AddScoped<ICallbackHandler, TasksCallbackHandler>();

        services.AddScoped<PendingActionMessageHandler>();
        services.AddScoped<IPendingInputHandler, ReminderTimeInputHandler>();
        services.AddScoped<IPendingInputHandler, TaskCreationPendingInputHandler>();

        return services;
    }
}
