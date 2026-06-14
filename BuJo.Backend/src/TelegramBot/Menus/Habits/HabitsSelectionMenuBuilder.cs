using BuJo.Contracts.V1.Habits;
using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Строит экран выбора привычки для отметки/статистики
/// </summary>
internal static class HabitsSelectionMenuBuilder
{
    public static MenuView BuildForMark(IReadOnlyList<HabitResponse> habits)
    {
        var lines = new List<string>
        {
            "📋 *Выберите привычку для отметки:*",
            string.Empty,
        };

        var keyboard = new List<List<InlineKeyboardButton>>();

        for (var i = 0; i < habits.Count; i++)
        {
            lines.Add($"{i + 1}. {habits[i].Name}");

            keyboard.Add([
                InlineKeyboardButton.WithCallbackData($"{i + 1}. {habits[i].Name}",
                    $"{HabitCallbacks.Prefix}:select_{habits[i].Id}"),
            ]);
        }

        keyboard.Add([
            InlineKeyboardButton.WithCallbackData("◀ Назад", HabitCallbacks.BackToList),
        ]);

        return new MenuView(string.Join("\n", lines), new InlineKeyboardMarkup(keyboard));
    }

    public static MenuView BuildForStats(IReadOnlyList<HabitResponse> habits)
    {
        var lines = new List<string>
        {
            "📊 *Выберите привычку для статистики:*",
            string.Empty,
        };

        var keyboard = new List<List<InlineKeyboardButton>>();

        for (var i = 0; i < habits.Count; i++)
        {
            lines.Add($"{i + 1}. {habits[i].Name}");
            
            keyboard.Add([
                InlineKeyboardButton.WithCallbackData($"{i + 1}. {habits[i].Name}",
                    $"{HabitCallbacks.Prefix}:stats_{habits[i].Id}"),
            ]);
        }

        keyboard.Add([
            InlineKeyboardButton.WithCallbackData("🏠 Назад", MenuCallbacks.HabitsList),
        ]);

        return new MenuView(string.Join("\n", lines), new InlineKeyboardMarkup(keyboard));
    }
}