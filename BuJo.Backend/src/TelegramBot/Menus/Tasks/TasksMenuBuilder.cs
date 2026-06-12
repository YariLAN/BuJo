using BuJo.Contracts.V1.Tasks;
using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Tasks;

/// <summary>
/// Сборщик экрана списка задач — встраивается в систему меню через MenuRenderer
/// </summary>
public static class TasksMenuBuilder
{
    public static MenuView Build(IReadOnlyList<TaskResponse> tasks)
    {
        if (tasks.Count == 0)
            return BuildEmpty();

        var lines = tasks.Select((t, i) =>
            $"{i + 1}. {(t.Status == TaskStatusDto.Done ? "✅" : "📌")} <b>{Escape(t.Title)}</b>\n" +
            $"   Статус: {t.Status}\n" +
            $"   Приоритет: {GetPriorityEmoji(t.Priority)} {t.Priority}\n" +
            $"   Дедлайн: {t.DueDate?.ToString("dd.MM.yyyy") ?? "—"}");

        var text = $"📋 <b>Ваши задачи:</b>\n\n{string.Join("\n\n", lines)}";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("➕ Создать задачу", TasksCallbacks.Create) },
            new[] { InlineKeyboardButton.WithCallbackData("⬅️ Назад", MenuCallbacks.Main) },
        });

        return new MenuView(text, markup);
    }

    private static MenuView BuildEmpty()
    {
        const string text = "📋 У вас пока нет задач.\nНажмите кнопку ниже, чтобы создать новую!";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("➕ Создать задачу", TasksCallbacks.Create) },
            new[] { InlineKeyboardButton.WithCallbackData("⬅️ Назад", MenuCallbacks.Main) },
        });

        return new MenuView(text, markup);
    }

    private static string GetPriorityEmoji(TaskPriorityDto priority) => priority switch
    {
        TaskPriorityDto.Low => "🟢",
        TaskPriorityDto.Medium => "🟡",
        TaskPriorityDto.High => "🟠",
        TaskPriorityDto.Critical => "🔴",
        _ => "⚪",
    };

    private static string Escape(string text) => text
        .Replace("&", "&")
        .Replace("<", "<")
        .Replace(">", ">");
}