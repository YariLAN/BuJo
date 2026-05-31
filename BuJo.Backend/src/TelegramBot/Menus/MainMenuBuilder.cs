using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus;

/// <summary>
/// Сборщик главного меню
/// </summary>
public static class MainMenuBuilder
{
    public static MenuView Build()
    {
        const string text = "🗂 BuJo — главное меню\nВыбери действие:";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("➕ Привычка", MenuCallbacks.HabitCreate),
                InlineKeyboardButton.WithCallbackData("➕ Задача", MenuCallbacks.TaskCreate),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📋 Привычки", MenuCallbacks.HabitsList),
                InlineKeyboardButton.WithCallbackData("📋 Задачи", MenuCallbacks.TasksList),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⚙️ Настройки", MenuCallbacks.Settings),
            },
        });

        return new MenuView(text, markup);
    }
}
