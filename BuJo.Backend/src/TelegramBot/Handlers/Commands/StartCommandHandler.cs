using BuJo.Application.Accounting;
using BuJo.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class StartCommandHandler(
    ITelegramBotClient botClient,
    IUserService userService,
    IMenuService menuService) : ICommandHandler
{
    public string Command => "/start";

    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        var telegramId = message.From!.Id.ToString();
        var chatId = message.Chat.Id;

        var existing = await userService.GetOrDefaultAsync(new GetUserQuery(null, telegramId), ct);

        if (existing?.Id is null)
        {
            var created = await userService.CreateAsync(
                new CreateUserCommand(telegramId, message.From!.Username),
                ct);

            var greeting = created.Name is null
                ? "Привет! 👋\nДобро пожаловать в BuJo — твой личный трекер задач и привычек."
                : $"Привет, {created.Name}! 👋\nДобро пожаловать в BuJo — твой личный трекер задач и привычек.";

            await botClient.SendMessage(chatId, greeting, cancellationToken: ct);

            await menuService.StartAsync(created.Id!.Value, chatId, ct);
            return;
        }

        await menuService.StartAsync(existing.Id.Value, chatId, ct);
    }
}
