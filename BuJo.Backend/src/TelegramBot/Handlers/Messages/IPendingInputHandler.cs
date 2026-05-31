using BuJo.Domain.Accounting;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages;

/// <summary>
/// Обработчик пользовательского текста, поступившего при активном PendingAction
/// </summary>
public interface IPendingInputHandler
{
    /// <summary>
    /// Поддерживает ли этот handler указанное ожидаемое действие
    /// </summary>
    bool CanHandle(PendingAction action);

    /// <summary>
    /// Обработать текстовое сообщение в контексте указанного ожидаемого действия
    /// </summary>
    Task HandleAsync(Guid userId, long chatId, PendingAction action, Message message, CancellationToken ct);
}
