using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class CreateTaskCommandHandler : ICommandHandler
{
    public string Command => "/create-task";
    
    public Task HandleAsync(Message message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}