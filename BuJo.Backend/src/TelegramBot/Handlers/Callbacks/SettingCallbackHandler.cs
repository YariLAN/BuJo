using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus.Settings;
using BuJo.TelegramBot.Services;
using BuJo.TelegramBot.Services.Settings;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class SettingCallbackHandler : CallbackHandlerBase
{
    private readonly IUserService userService;
    private readonly ISettingsMenuService settingsMenuService;
    private readonly ILogger<SettingCallbackHandler> logger;

    public SettingCallbackHandler(
        ITelegramBotClient botClient, 
        IUserService userService, 
        ISettingsMenuService settingsMenuService, 
        ILogger<SettingCallbackHandler> logger) : base(botClient, userService, logger)
    {
        this.userService = userService;
        this.settingsMenuService = settingsMenuService;
        this.logger = logger;
    }

    public override string Prefix => SettingCallbacks.Prefix;

    protected override async Task HandleCallbackAsync(Guid userId, CallbackQuery callbackQuery, CancellationToken ct = default)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var data = callbackQuery.Data!;
        
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