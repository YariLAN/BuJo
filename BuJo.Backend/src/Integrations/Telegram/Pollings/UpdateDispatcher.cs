using BuJo.Integrations.Telegram.Handlers;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuJo.Integrations.Telegram.Pollings;

internal sealed class UpdateDispatcher(
    IEnumerable<ICommandHandler> commandHandlers,
    ILogger<UpdateDispatcher> logger)
{
    public async Task DispatchAsync(Update update, CancellationToken ct)
    {
        switch (update.Type)
        {
            case UpdateType.Message when update.Message?.Text?.StartsWith("/") == true:
                await HandleCommandAsync(update.Message, ct);
                break;
            
            default:
                logger.LogWarning("Unhandled update type: {Type}", update.Type);
                break;
        }
    }

    private async Task HandleCommandAsync(Message message, CancellationToken ct)
    {
        var command = message.Text!.Split(' ')[0].ToLower();

        var handler = commandHandlers.FirstOrDefault(h => h.Command == command);

        if (handler is null)
        {
            logger.LogWarning("Unknown command: {Command}", command);
            return;
        }

        await handler.HandleAsync(message, ct);
    }
}