using BuJo.Contracts.V1.Habits;
using BuJo.TelegramBot.Menus;
using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Строит подменю конкретной привычки (отметить/статистика/назад)
/// </summary>
internal static class HabitMenuBuilder
{
    public static MenuView Build(HabitResponse habit)
    {
        var lines = new List<string>
        {
            $"📌 *{habit.Name}*",
            string.Empty,
            "Что делаем?",
        };

        var keyboard = new List<List<InlineKeyboardButton>>
        {
            ([
                InlineKeyboardButton.WithCallbackData("✅ Отметить", HabitCallbacks.MarkToday),
            ]),
            ([
                InlineKeyboardButton.WithCallbackData("📅 Другая дата", HabitCallbacks.MarkOtherDate),
            ]),
            ([
                InlineKeyboardButton.WithCallbackData("❌ Пропустить", HabitCallbacks.MarkSkip),
            ]),
            ([
                InlineKeyboardButton.WithCallbackData("📊 Статистика", HabitCallbacks.ViewStats),
            ]),
            ([
                InlineKeyboardButton.WithCallbackData("🏠 Назад", MenuCallbacks.HabitsList),
            ])
        };

        return new MenuView(string.Join("\n", lines), new InlineKeyboardMarkup(keyboard));
    }

    public static MenuView BuildMarkResult(string habitName, DateOnly date, bool isCompleted)
    {
        var emoji = isCompleted ? "✅" : "❌";
        var status = isCompleted ? "выполнено" : "не выполнено";
        var text = $"{emoji} *{habitName}* — {date:dd.MM.yyyy}: {status}";

        var keyboard = new List<List<InlineKeyboardButton>>
        {
            ([InlineKeyboardButton.WithCallbackData("🏠 Назад", MenuCallbacks.HabitsList)]),
        };

        return new MenuView(text, new InlineKeyboardMarkup(keyboard));
    }
}