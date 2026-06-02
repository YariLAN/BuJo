using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages;

/// <summary>
/// Диспатчер текстовых сообщений: маршрутизирует входящий текст в IPendingInputHandler
/// по текущему PendingAction пользователя
/// </summary>
public sealed class PendingActionMessageHandler(
    IUserService userService,
    IUserBotStateService userBotStateService,
    IEnumerable<IPendingInputHandler> handlers,
    ILogger<PendingActionMessageHandler> logger)
{
    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        if (message.From is null)
            return;

        var telegramId = message.From.Id.ToString();
        var user = await userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
        {
            logger.LogWarning("Pending text input from unknown user {TelegramId}", telegramId);
            return;
        }

        var userId = user.Id.Value;
        var chatId = message.Chat.Id;

        var state = await userBotStateService.GetOrCreateAsync(userId, chatId, ct);
        if (state.PendingAction == PendingAction.None)
        {
            logger.LogWarning("PendingActionMessageHandler invoked but PendingAction is None for user {UserId}", userId);
            return;
        }

        var handler = handlers.FirstOrDefault(h => h.CanHandle(state.PendingAction));
        if (handler is null)
        {
            logger.LogWarning("No IPendingInputHandler registered for PendingAction {PendingAction}", state.PendingAction);
            return;
        }

        await handler.HandleAsync(userId, chatId, state.PendingAction, message, ct);
    }
}
