using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus;

/// <summary>
/// Сборщик главного экрана настроек (хаб с категориями)
/// </summary>
public static class SettingsMenuBuilder
{
    public static MenuView Build()
    {
        const string text = "⚙️ Настройки\nВыбери раздел:";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔔 Напоминания", MenuCallbacks.SettingsReminders),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Назад", MenuCallbacks.Main),
            },
        });

        return new MenuView(text, markup);
    }
}
