using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Сборщик экрана подтверждения создания привычки
/// </summary>
public static class HabitCreatedMenuBuilder
{
    public static MenuView Build(string habitName)
    {
        var text = $"✅ Привычка «{habitName}» добавлена!";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📋 Мои привычки", MenuCallbacks.HabitsList),
                InlineKeyboardButton.WithCallbackData("🏠 Назад", MenuCallbacks.Main),
            },
        });

        return new MenuView(text, markup);
    }
}
