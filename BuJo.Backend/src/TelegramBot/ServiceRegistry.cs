using BuJo.TelegramBot.Handlers;
using BuJo.TelegramBot.Handlers.Commands;
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
        services.AddScoped<ICommandHandler, StartCommandHandler>();

        return services;
    }
}