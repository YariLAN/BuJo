using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using Telegram.Bot;

namespace BuJo.TelegramBot.Services.Tasks;

public sealed class TasksMenuService(ITelegramBotClient botClient, IUserBotStateService userBotStateService) : ITasksMenuService
{
    public async Task OpenCreateAsync(Guid userId, long chatId, CancellationToken ct)
    {
        await userBotStateService.SetPendingActionAsync(
            userId, chatId, PendingAction.AwaitingTaskTitle, null, ct);

        // Delete active menu (if any) — this screen exits the menu flow
        var state = await userBotStateService.GetOrCreateAsync(userId, chatId, ct);
        if (state.LastMenuMessageId is not null)
        {
            try { await botClient.DeleteMessage(chatId, state.LastMenuMessageId.Value, ct); }
            catch { /* best-effort */ }
        }

        await botClient.SendMessage(
            chatId,
            "📝 Введите название задачи:",
            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.ForceReplyMarkup(),
            cancellationToken: ct);
    }
}