using BuJo.Domain.Accounting;

namespace BuJo.TelegramBot.Menus.Settings;

public static class SettingCallbacks
{
    /// <summary>
    /// Префикс домена меню — используется UpdateDispatcher для маршрутизации CallbackQuery
    /// </summary>
    public const string Prefix = "settings";
    
    public const string SettingsReminders = Prefix + ":reminders";

    public const string SettingsRemindersMorning = SettingsReminders + ":morning";

    public const string SettingsRemindersEvening = SettingsReminders + ":evening";

    public const string SettingsRemindersMorningCustom = SettingsRemindersMorning + ":custom";

    public const string SettingsRemindersEveningCustom = SettingsRemindersEvening + ":custom";
    
    /// <summary>
    /// Построить callback_data для установки времени напоминания по пресету
    /// </summary>
    public static string SettingsRemindersSet(ReminderKind kind, TimeOnly time)
        => $"{SettingsRemindersPicker(kind)}:set:{time:HH\\:mm}";
    
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