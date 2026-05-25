using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BuJo.Integrations.Telegram;

public static class ServiceRegistry
{
    public static IServiceCollection AddTelegram(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>();

        return services;
    }
}