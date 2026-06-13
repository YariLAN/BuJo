using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Сборщик экрана ввода названия новой привычки
/// </summary>
public static class HabitCreatePromptBuilder
{
    public static MenuView Build(string? error = null)
    {
        var text = error is null
            ? "✏️ Введите название привычки:"
            : $"⚠️ {error}";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("❌ Отмена", MenuCallbacks.Main),
            },
        });

        return new MenuView(text, markup);
    }
}
