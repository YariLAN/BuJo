using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuJo.Integrations.Telegram.Pollings;

internal sealed class BotWatcherWorker(
    ITelegramBotClient botClient,
    IServiceProvider serviceProvider,
    ILogger<BotWatcherWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
            DropPendingUpdates = true  // игнорировать сообщения пока бот был оффлайн
        };

        logger.LogInformation("Telegram bot polling started");

        await botClient.ReceiveAsync(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: ct);
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<UpdateDispatcher>();
        
        try
        {
            await dispatcher.DispatchAsync(update, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling update {UpdateId}", update.Id);
        }
    }

    private Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception ex,
        CancellationToken ct)
    {
        logger.LogError(ex, "Telegram polling error");
        return Task.CompletedTask;
    }
}