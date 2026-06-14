using BuJo.Contracts.V1.Habits;
using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Сборщик экрана списка привычек
/// </summary>
public static class HabitsListMenuBuilder
{
    public static MenuView Build(IReadOnlyList<HabitResponse> habits)
    {
        string text;

        if (habits.Count == 0)
        {
            text = "📋 У вас пока нет привычек.";
        }
        else
        {
            var lines = habits.Select((h, i) => $"{i + 1}. {h.Name}");
            text = "📋 Выберите привычку (введите номер):\n" + string.Join("\n", lines);
        }

        var buttons = new List<List<InlineKeyboardButton>>
        {
            ([
                InlineKeyboardButton.WithCallbackData("➕ Добавить", HabitCallbacks.Add),
            ])
        };

        if (habits.Count > 0)
        {
            buttons.Add([
                InlineKeyboardButton.WithCallbackData("📊 Статистика", HabitCallbacks.StatsSelect),
            ]);
        }

        buttons.Add([
            InlineKeyboardButton.WithCallbackData("🏠 Назад", MenuCallbacks.Main),
        ]);

        return new MenuView(text, new InlineKeyboardMarkup(buttons));
    }
}
