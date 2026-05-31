using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus;
using BuJo.TelegramBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class MenuCallbackHandler(
    IUserService userService,
    IMenuService menuService,
    ISettingsMenuService settingsMenuService,
    ITelegramBotClient botClient,
    ILogger<MenuCallbackHandler> logger) : ICallbackHandler
{
    public string Prefix => MenuCallbacks.Prefix;

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
            case MenuCallbacks.Main:
                await menuService.OpenMainAsync(userId, chatId, ct);
                return;

            case MenuCallbacks.Settings:
                await menuService.OpenSettingsAsync(userId, chatId, ct);
                return;

            case MenuCallbacks.SettingsReminders:
                await settingsMenuService.OpenRemindersAsync(userId, chatId, ct);
                return;

            case MenuCallbacks.SettingsRemindersMorning:
                await settingsMenuService.OpenTimePickerAsync(userId, chatId, ReminderKind.Morning, ct);
                return;

            case MenuCallbacks.SettingsRemindersEvening:
                await settingsMenuService.OpenTimePickerAsync(userId, chatId, ReminderKind.Evening, ct);
                return;

            case MenuCallbacks.SettingsRemindersMorningCustom:
                await settingsMenuService.OpenCustomTimePromptAsync(userId, chatId, ReminderKind.Morning, ct: ct);
                return;

            case MenuCallbacks.SettingsRemindersEveningCustom:
                await settingsMenuService.OpenCustomTimePromptAsync(userId, chatId, ReminderKind.Evening, ct: ct);
                return;

            case MenuCallbacks.HabitsList:
                await menuService.OpenStubAsync(userId, chatId, "Привычки", ct);
                return;

            case MenuCallbacks.HabitCreate:
                await menuService.OpenStubAsync(userId, chatId, "Создание привычки", ct);
                return;

            case MenuCallbacks.TasksList:
                await menuService.OpenStubAsync(userId, chatId, "Задачи", ct);
                return;

            case MenuCallbacks.TaskCreate:
                await menuService.OpenStubAsync(userId, chatId, "Создание задачи", ct);
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
    /// Распарсить callback_data вида menu:settings:reminders:{morning|evening}:set:HH:MM
    /// </summary>
    private static bool TryParseSetReminder(string data, out ReminderKind kind, out TimeOnly time)
    {
        kind = default;
        time = default;

        var parts = data.Split(':');
        if (parts.Length != 7
            || parts[0] != "menu"
            || parts[1] != "settings"
            || parts[2] != "reminders"
            || parts[4] != "set")
        {
            return false;
        }

        switch (parts[3])
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

        return TimeOnly.TryParseExact($"{parts[5]}:{parts[6]}", "HH:mm", out time);
    }
}
