using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.Integrations.Telegram.Handlers;

internal sealed class StartCommandHandler(
    ITelegramBotClient botClient) : ICommandHandler
{
    public string Command => "/start";

    public async Task HandleAsync(Message message, CancellationToken ct)
        => await botClient.SendMessage(message.Chat.Id, "Привет! BuJo работает 🎯", cancellationToken: ct);
}

public interface ICommandHandler
{
    string Command { get; }
    Task HandleAsync(Message message, CancellationToken ct);
}