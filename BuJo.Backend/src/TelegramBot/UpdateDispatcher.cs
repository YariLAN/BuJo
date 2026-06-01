using BuJo.TelegramBot.Handlers;
using BuJo.TelegramBot.Handlers.Messages;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuJo.TelegramBot;

internal sealed class UpdateDispatcher(
    IEnumerable<ICommandHandler> commandHandlers,
    IEnumerable<ICallbackHandler> callbackHandlers,
    PendingActionMessageHandler pendingActionMessageHandler,
    ILogger<UpdateDispatcher> logger)
{
    public async Task DispatchAsync(Update update, CancellationToken ct)
    {
        switch (update.Type)
        {
            case UpdateType.Message when update.Message?.Text?.StartsWith('/') == true:
                await HandleCommandAsync(update.Message, ct);
                break;

            case UpdateType.Message when update.Message?.Text is not null:
                await pendingActionMessageHandler.HandleAsync(update.Message, ct);
                break;

            case UpdateType.CallbackQuery when update.CallbackQuery?.Data is not null:
                await HandleCallbackAsync(update.CallbackQuery, ct);
                break;

            default:
                logger.LogWarning("Unhandled update type: {Type}", update.Type);
                break;
        }
    }

    private async Task HandleCommandAsync(Message message, CancellationToken ct)
    {
        var rawCommand = message.Text!.Split(' ')[0].ToLower();
        
        var command = rawCommand.Split('@')[0];

        var handler = commandHandlers.FirstOrDefault(h => h.Command == command);

        if (handler is null)
        {
            logger.LogWarning("Unknown command: {Command}", command);
            return;
        }

        await handler.HandleAsync(message, ct);
    }

    private async Task HandleCallbackAsync(CallbackQuery callback, CancellationToken ct)
    {
        var prefix = callback.Data!.Split(':')[0];

        var handler = callbackHandlers.FirstOrDefault(h => h.Prefix == prefix);

        if (handler is null)
        {
            logger.LogWarning("Unknown callback prefix {Prefix} (data: {Data})", prefix, callback.Data);
            return;
        }

        await handler.HandleAsync(callback, ct);
    }
}
