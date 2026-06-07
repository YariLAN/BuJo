using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Settings;

/// <summary>
/// Сборщик экрана настроек напоминаний — текущие времена + кнопки выбора утра/вечера
/// </summary>
public static class RemindersSettingsMenuBuilder
{
    public static MenuView Build(TimeOnly? morningTime, TimeOnly? eveningTime)
    {
        var text =
            $"""
            🔔 Настройки напоминаний

            Утро: {"\t"}{Format(morningTime)}
            Вечер: {"\t"}{Format(eveningTime)}
            """;

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🌅  Утреннее", SettingCallbacks.SettingsRemindersMorning),
                InlineKeyboardButton.WithCallbackData("🌙  Вечернее", SettingCallbacks.SettingsRemindersEvening),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Назад", MenuCallbacks.Settings),
            },
        });

        return new MenuView(text, markup);
    }

    private static string Format(TimeOnly? time)
        => time is null ? "не задано" : $"{time:HH\\:mm}";
}
