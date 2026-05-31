using BuJo.Application.Accounting;
using BuJo.TelegramBot.Menus;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BuJo.TelegramBot.Services;

/// <summary>
/// Внутренний helper для отображения меню: редактирует активное сообщение либо пересоздаёт его
/// </summary>
internal sealed class MenuRenderer(
    ITelegramBotClient botClient,
    IUserBotStateService userBotStateService,
    ILogger<MenuRenderer> logger)
{
    /// <summary>
    /// Отредактировать активное сообщение-меню. Если активного нет — отправить новое.
    /// </summary>
    public async Task EditAsync(Guid userId, long chatId, MenuView view, CancellationToken ct)
    {
        var state = await userBotStateService.GetOrCreateAsync(userId, ct);

        if (state.LastMenuMessageId is null)
        {
            logger.LogWarning("Active menu message not found for user {UserId}, sending new", userId);
            await SendAndSaveAsync(userId, chatId, view, ct);
            return;
        }

        await botClient.EditMessageText(
            chatId: chatId,
            messageId: state.LastMenuMessageId.Value,
            text: view.Text,
            replyMarkup: view.Markup,
            cancellationToken: ct);
    }

    /// <summary>
    /// Удалить старое сообщение-меню (если есть) и отправить новое
    /// </summary>
    public async Task RecreateAsync(Guid userId, long chatId, MenuView view, CancellationToken ct)
    {
        var state = await userBotStateService.GetOrCreateAsync(userId, ct);

        if (state.LastMenuMessageId is not null)
        {
            try
            {
                await botClient.DeleteMessage(chatId, state.LastMenuMessageId.Value, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete previous menu message {MessageId} for user {UserId}",
                    state.LastMenuMessageId.Value, userId);
            }
        }

        await SendAndSaveAsync(userId, chatId, view, ct);
    }

    private async Task SendAndSaveAsync(Guid userId, long chatId, MenuView view, CancellationToken ct)
    {
        var sent = await botClient.SendMessage(
            chatId: chatId,
            text: view.Text,
            replyMarkup: view.Markup,
            cancellationToken: ct);

        await userBotStateService.UpdateLastMenuMessageAsync(userId, sent.MessageId, ct);
    }
}
