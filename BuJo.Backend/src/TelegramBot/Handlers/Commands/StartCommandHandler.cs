using BuJo.Application.Accounting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class StartCommandHandler(ITelegramBotClient botClient, IUserService userService) : ICommandHandler
{
    public string Command => "/start";

    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        var telegramId = message.From!.Id.ToString();
        var exist = await userService.GetOrDefaultAsync(new GetUserQuery(null, telegramId), ct);

        if (exist is not null)
        {
            await botClient.SendMessage(message.Chat.Id, "С возвращением!", cancellationToken: ct);
            return;
        }

        var user = await userService.CreateAsync(new CreateUserCommand(telegramId, message.From!.Username), ct);
        
        await botClient.SendMessage(message.Chat.Id,
            $"Привет, {user.Name}! 👋\nДобро пожаловать в BuJo — твой личный трекер задач и привычек.",
            cancellationToken: ct);
    }
}