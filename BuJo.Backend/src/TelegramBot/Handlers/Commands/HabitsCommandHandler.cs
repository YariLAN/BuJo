using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class HabitsCommandHandler : ICommandHandler
{
    public string Command => "/habits";
    
    public Task HandleAsync(Message message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}