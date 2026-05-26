using BuJo.Integrations.Telegram.Handlers;
using BuJo.Integrations.Telegram.Pollings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BuJo.Integrations.Telegram;

public static class ServiceRegistry
{
    public static IServiceCollection AddTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TelegramOptions>().BindConfiguration(TelegramOptions.SectionName);
        
        var options = configuration
                .GetRequiredSection(TelegramOptions.SectionName)
                .Get<TelegramOptions>() 
            ?? throw new ArgumentNullException();
        
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(options.TokenRequired));
        
        services.AddHostedService<BotWatcherWorker>();

        services.AddScoped<UpdateDispatcher>();
        services.AddScoped<ICommandHandler, StartCommandHandler>();

        return services;
    }
}