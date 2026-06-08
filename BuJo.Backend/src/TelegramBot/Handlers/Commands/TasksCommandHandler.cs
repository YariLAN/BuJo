using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class TasksCommandHandler : ICommandHandler
{
    public string Command => "/tasks";
    
    public Task HandleAsync(Message message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}