using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Services;
using BuJo.TelegramBot.Services.Settings;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages;

/// <summary>
/// Обрабатывает ручной ввод времени напоминания (HH:MM) от пользователя
/// </summary>
public sealed class ReminderTimeInputHandler(
    IUserService userService,
    ISettingsMenuService settingsMenuService,
    ITelegramBotClient botClient,
    ILogger<ReminderTimeInputHandler> logger) : IPendingInputHandler
{
    public bool CanHandle(PendingAction action)
        => action is PendingAction.AwaitingMorningTime or PendingAction.AwaitingEveningTime;

    public async Task HandleAsync(Guid userId, long chatId, PendingAction action, Message message, CancellationToken ct)
    {
        var kind = action switch
        {
            PendingAction.AwaitingMorningTime => ReminderKind.Morning,
            PendingAction.AwaitingEveningTime => ReminderKind.Evening,
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null),
        };

        var text = (message.Text ?? string.Empty).Trim();

        if (!TimeOnly.TryParseExact(text, "HH:mm", out var time))
        {
            await TryDeleteAsync(chatId, message.MessageId, ct);
            await settingsMenuService.OpenCustomTimePromptAsync(userId, chatId, kind, withError: true, ct: ct);
            return;
        }

        await userService.SetReminderAsync(userId, kind, time, ct);
        await TryDeleteAsync(chatId, message.MessageId, ct);
        await settingsMenuService.OpenRemindersAsync(userId, chatId, ct);
    }

    private async Task TryDeleteAsync(long chatId, int messageId, CancellationToken ct)
    {
        try
        {
            await botClient.DeleteMessage(chatId, messageId, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete user message {MessageId} in chat {ChatId}", messageId, chatId);
        }
    }
}
