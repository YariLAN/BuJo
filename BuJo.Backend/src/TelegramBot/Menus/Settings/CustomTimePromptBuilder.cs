using BuJo.Domain.Accounting;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Settings;

/// <summary>
/// Сборщик экрана ручного ввода времени напоминания
/// </summary>
public static class CustomTimePromptBuilder
{
    public static MenuView Build(ReminderKind kind, bool withError = false)
    {
        var icon = kind == ReminderKind.Morning ? "🌅" : "🌙";

        var text = withError
            ? $"❌ Неверный формат.\n{icon} Введи время в формате HH:MM (например, 09:30)."
            : $"{icon} Введи время в формате HH:MM (например, 09:30).";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Отмена", SettingCallbacks.SettingsRemindersPicker(kind)),
            },
        });

        return new MenuView(text, markup);
    }
}
