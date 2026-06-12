using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Application.Tasks.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class TasksCommandHandler(
    ITaskService taskService,
    IUserService userService,
    ITelegramBotClient botClient) : ICommandHandler
{
    public string Command => "/tasks";
    
    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        if (message.From is null)
            return;

        var telegramId = message.From.Id.ToString();
        var user = await userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
        {
            await botClient.SendMessage(message.Chat.Id, "❌ Пользователь не найден. Используйте /start для регистрации.", cancellationToken: ct);
            return;
        }

        var userId = user.Id.Value;
        var tasks = await taskService.GetTasksAsync(userId, ct);

        if (tasks.Count == 0)
        {
            await botClient.SendMessage(
                message.Chat.Id,
                "📋 У вас пока нет задач. Нажмите кнопку ниже, чтобы создать новую!",
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("➕ Создать задачу", "tasks:create")),
                cancellationToken: ct);
            return;
        }

        var taskList = string.Join("\n\n", tasks.Select((t, i) =>
            $"{i + 1}. {(t.Status == Contracts.V1.Tasks.TaskStatusDto.Done ? "✅" : "📌")} {t.Title}\n" +
            $"   Статус: {t.Status}\n" +
            $"   Приоритет: {GetPriorityEmoji(t.Priority)} {t.Priority}\n" +
            $"   Дедлайн: {t.DueDate?.ToString("dd.MM.yyyy") ?? "—"}"));

        await botClient.SendMessage(
            message.Chat.Id,
            $"📋 <b>Ваши задачи:</b>\n\n{taskList}",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("➕ Создать задачу", "tasks:create")),
            cancellationToken: ct);
    }

    private static string GetPriorityEmoji(Contracts.V1.Tasks.TaskPriorityDto priority) => priority switch
    {
        Contracts.V1.Tasks.TaskPriorityDto.Low => "🟢",
        Contracts.V1.Tasks.TaskPriorityDto.Medium => "🟡",
        Contracts.V1.Tasks.TaskPriorityDto.High => "🟠",
        Contracts.V1.Tasks.TaskPriorityDto.Critical => "🔴",
        _ => "⚪",
    };
}