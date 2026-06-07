using BuJo.Domain.Accounting;

namespace BuJo.TelegramBot.Menus.Settings;

public static class SettingCallbacks
{
    /// <summary>
    /// Префикс домена меню — используется UpdateDispatcher для маршрутизации CallbackQuery
    /// </summary>
    public const string Prefix = "settings";
    
    public const string SettingsReminders = "settings:reminders";

    public const string SettingsRemindersMorning = "settings:reminders:morning";

    public const string SettingsRemindersEvening = "settings:reminders:evening";

    public const string SettingsRemindersMorningCustom = "settings:reminders:morning:custom";

    public const string SettingsRemindersEveningCustom = "settings:reminders:evening:custom";
    
    /// <summary>
    /// Построить callback_data для установки времени напоминания по пресету
    /// </summary>
    public static string SettingsRemindersSet(ReminderKind kind, TimeOnly time)
        => $"{SettingsReminders}:{KindSlug(kind)}:set:{time:HH\\:mm}";
    
    private static string KindSlug(ReminderKind kind) => kind switch
    {
        ReminderKind.Morning => "morning",
        ReminderKind.Evening => "evening",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };
    
    /// <summary>
    /// Получить callback_data выбора времени для указанного типа напоминания
    /// </summary>
    public static string SettingsRemindersPicker(ReminderKind kind)
        => kind switch
        {
            ReminderKind.Morning => SettingsRemindersMorning,
            ReminderKind.Evening => SettingsRemindersEvening,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

    /// <summary>
    /// Получить callback_data «свой вариант» для указанного типа напоминания
    /// </summary>
    public static string SettingsRemindersCustom(ReminderKind kind)
        => kind switch
        {
            ReminderKind.Morning => SettingsRemindersMorningCustom,
            ReminderKind.Evening => SettingsRemindersEveningCustom,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
}