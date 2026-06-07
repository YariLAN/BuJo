using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus;
using BuJo.TelegramBot.Menus.Settings;
using BuJo.TelegramBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class SettingCallbackHandler(
    IUserService userService,
    ISettingsMenuService settingsMenuService,
    ITelegramBotClient botClient,
    ILogger<MenuCallbackHandler> logger) : ICallbackHandler
{
    public string Prefix => SettingCallbacks.Prefix;
    
    public async Task HandleAsync(CallbackQuery callback, CancellationToken ct)
    {
        try
        {
            await DispatchAsync(callback, ct);
        }
        finally
        {
            try
            {
                await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to answer callback query {CallbackId}", callback.Id);
            }
        }
    }

    private async Task DispatchAsync(CallbackQuery callback, CancellationToken ct)
    {
        if (callback.Message is null || callback.Data is null)
        {
            logger.LogWarning("Callback {CallbackId} has no message or data", callback.Id);
            return;
        }

        var telegramId = callback.From.Id.ToString();
        var user = await userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
        {
            await botClient.AnswerCallbackQuery(
                callback.Id,
                text: "Сначала отправь /start",
                showAlert: true,
                cancellationToken: ct);
            return;
        }

        var userId = user.Id.Value;
        var chatId = callback.Message.Chat.Id;
        var data = callback.Data;

        switch (data)
        {
            case SettingCallbacks.SettingsReminders:
                await settingsMenuService.OpenRemindersAsync(userId, chatId, ct);
                return;
            
            case SettingCallbacks.SettingsRemindersMorning:
                await settingsMenuService.OpenTimePickerAsync(userId, chatId, ReminderKind.Morning, ct);
                return;
            
            case SettingCallbacks.SettingsRemindersEvening:
                await settingsMenuService.OpenTimePickerAsync(userId, chatId, ReminderKind.Evening, ct);
                return;
            
            case SettingCallbacks.SettingsRemindersMorningCustom:
                await settingsMenuService.OpenCustomTimePromptAsync(userId, chatId, ReminderKind.Morning, ct: ct);
                return;
            
            case SettingCallbacks.SettingsRemindersEveningCustom:
                await settingsMenuService.OpenCustomTimePromptAsync(userId, chatId, ReminderKind.Evening, ct: ct);
                return;
        }

        if (TryParseSetReminder(data, out var kind, out var time))
        {
            await userService.SetReminderAsync(userId, kind, time, ct);
            await settingsMenuService.OpenRemindersAsync(userId, chatId, ct);
            return;
        }

        logger.LogWarning("Unknown menu callback data: {Data}", data);
    }

    /// <summary>
    /// Распарсить callback_data вида settings:reminders:{morning|evening}:set:HH:MM
    /// </summary>
    private static bool TryParseSetReminder(string data, out ReminderKind kind, out TimeOnly time)
    {
        kind = default;
        time = default;

        var parts = data.Split(':');
        if (parts.Length != 6
            || parts[0] != "settings"
            || parts[1] != "reminders"
            || parts[3] != "set")
        {
            return false;
        }

        switch (parts[2])
        {
            case "morning":
                kind = ReminderKind.Morning;
                break;
            case "evening":
                kind = ReminderKind.Evening;
                break;
            default:
                return false;
        }

        return TimeOnly.TryParseExact($"{parts[4]}:{parts[5]}", "HH:mm", out time);
    }
}