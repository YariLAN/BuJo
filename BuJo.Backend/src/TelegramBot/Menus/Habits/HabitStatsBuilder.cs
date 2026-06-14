using BuJo.Contracts.V1.Habits;
using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Строит экран статистики привычки
/// </summary>
internal static class HabitStatsBuilder
{
    public static MenuView Build(HabitStatsResponse stats, string habitName)
    {
        var lines = new List<string>
        {
            $"📊 *Статистика: {habitName}*",
            string.Empty,
            $"🔥 Текущая серия: *{stats.CurrentStreak}* дн.",
            $"🏆 Лучшая серия: *{stats.BestStreak}* дн.",
            $"📈 Выполнение: *{stats.CompletionRate}%*",
            $"✅ Всего отметок: *{stats.TotalCompleted}*",
            string.Empty,
            "📅 *Календарь:",
        };

        // Show last 7 days as mini calendar
        var calendarDays = stats.CalendarDays;
        if (calendarDays.Count > 0)
        {
            var lastDays = calendarDays.TakeLast(7).ToList();
            var calendarLine = string.Join(" ", lastDays.Select(d => d.IsCompleted ? "🟢" : "⚪"));
            lines.Add(calendarLine);
        }

        if (stats.MonthlyStats.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("📆 *По месяцам:*");
            foreach (var ms in stats.MonthlyStats)
            {
                var monthName = new DateTime(ms.Year, ms.Month, 1).ToString("MMM yyyy");
                lines.Add($"  {monthName}: {ms.CompletedDays}/{ms.TotalDays}");
            }
        }

        var keyboard = new List<List<InlineKeyboardButton>>
        {
            ([InlineKeyboardButton.WithCallbackData("🏠 Назад", MenuCallbacks.HabitsList)]),
        };

        return new MenuView(string.Join("\n", lines), new InlineKeyboardMarkup(keyboard));
    }
}