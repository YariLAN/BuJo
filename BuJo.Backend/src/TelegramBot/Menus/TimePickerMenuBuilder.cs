using BuJo.Domain.Accounting;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus;

/// <summary>
/// Сборщик меню выбора времени напоминания — пресеты + «Свой вариант» + «Назад»
/// </summary>
public static class TimePickerMenuBuilder
{
    private static readonly TimeOnly[] MorningPresets =
    [
        new(7, 0),
        new(8, 0),
        new(9, 0),
        new(10, 0),
    ];

    private static readonly TimeOnly[] EveningPresets =
    [
        new(19, 0),
        new(20, 0),
        new(21, 0),
        new(22, 0),
    ];

    public static MenuView Build(ReminderKind kind)
    {
        var (icon, label, presets) = kind switch
        {
            ReminderKind.Morning => ("🌅", "утреннего", MorningPresets),
            ReminderKind.Evening => ("🌙", "вечернего", EveningPresets),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        var text = $"{icon} Выбери время {label} напоминания:";

        var presetButtons = presets
            .Select(time => InlineKeyboardButton.WithCallbackData(
                text: $"{time:HH\\:mm}",
                callbackData: MenuCallbacks.SettingsRemindersSet(kind, time)))
            .ToArray();

        var markup = new InlineKeyboardMarkup(new[]
        {
            presetButtons,
            new[]
            {
                InlineKeyboardButton.WithCallbackData("✏️ Свой вариант", MenuCallbacks.SettingsRemindersCustom(kind)),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Назад", MenuCallbacks.SettingsReminders),
            },
        });

        return new MenuView(text, markup);
    }
}
