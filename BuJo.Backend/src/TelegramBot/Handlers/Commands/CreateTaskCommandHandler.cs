using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class CreateTaskCommandHandler(
    IUserService userService,
    IUserBotStateService userBotStateService,
    ITelegramBotClient botClient) : ICommandHandler
{
    public string Command => "/create-task";
    
    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        if (message.From is null)
            return;

        var telegramId = message.From.Id.ToString();
        var user = await userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
        {
            await botClient.SendMessage(message.Chat.Id, "❌ Пользователь не найден. Используйте /start для регистрации.", cancellationToken: ct);
            return;
        }

        var userId = user.Id.Value;
        var chatId = message.Chat.Id;

        await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.AwaitingTaskTitle, null, ct);

        await botClient.SendMessage(
            chatId,
            "📝 Введите название задачи:",
            replyMarkup: new ForceReplyMarkup(),
            cancellationToken: ct);
    }
}