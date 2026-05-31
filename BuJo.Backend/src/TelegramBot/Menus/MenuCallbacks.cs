using BuJo.Domain.Accounting;

namespace BuJo.TelegramBot.Menus;

/// <summary>
/// Константы и фабрики значений callback_data для inline-кнопок меню
/// </summary>
public static class MenuCallbacks
{
    /// <summary>
    /// Префикс домена меню — используется UpdateDispatcher для маршрутизации CallbackQuery
    /// </summary>
    public const string Prefix = "menu";

    public const string Main = "menu:main";

    public const string Settings = "menu:settings";

    public const string SettingsReminders = "menu:settings:reminders";

    public const string SettingsRemindersMorning = "menu:settings:reminders:morning";

    public const string SettingsRemindersEvening = "menu:settings:reminders:evening";

    public const string SettingsRemindersMorningCustom = "menu:settings:reminders:morning:custom";

    public const string SettingsRemindersEveningCustom = "menu:settings:reminders:evening:custom";

    public const string HabitsList = "menu:habits";

    public const string HabitCreate = "menu:habit:create";

    public const string TasksList = "menu:tasks";

    public const string TaskCreate = "menu:task:create";

    /// <summary>
    /// Построить callback_data для установки времени напоминания по пресету
    /// </summary>
    public static string SettingsRemindersSet(ReminderKind kind, TimeOnly time)
        => $"menu:settings:reminders:{KindSlug(kind)}:set:{time:HH\\:mm}";

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

    private static string KindSlug(ReminderKind kind) => kind switch
    {
        ReminderKind.Morning => "morning",
        ReminderKind.Evening => "evening",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };
}
