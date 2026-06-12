using System.Text.Json;
using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Application.Tasks.Abstractions;
using BuJo.Contracts.V1.Tasks;
using BuJo.Domain.Accounting;
using BuJo.Domain.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace BuJo.TelegramBot.Handlers.Messages;

internal sealed class TaskCreationPendingInputHandler(
    ITaskService taskService,
    IUserBotStateService userBotStateService,
    ITelegramBotClient botClient,
    ILogger<TaskCreationPendingInputHandler> logger) : IPendingInputHandler
{
    private static readonly PendingAction[] HandledActions =
    [
        PendingAction.AwaitingTaskTitle,
        PendingAction.AwaitingTaskDescription,
        PendingAction.AwaitingTaskDueDate,
    ];

    public bool CanHandle(PendingAction action) => HandledActions.Contains(action);

    public async Task HandleAsync(Guid userId, long chatId, PendingAction action, Message message, CancellationToken ct)
    {
        if (message.Text is null)
            return;

        var state = await userBotStateService.GetOrCreateAsync(userId, chatId, ct);
        var payload = DeserializePayload(state.PendingPayload);

        switch (action)
        {
            case PendingAction.AwaitingTaskTitle:
                payload.Title = message.Text;
                await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.AwaitingTaskDescription, SerializePayload(payload), ct);
                await botClient.SendMessage(chatId, "Введите описание задачи (или отправьте \"-\" чтобы пропустить):", replyMarkup: new ForceReplyMarkup(), cancellationToken: ct);
                break;

            case PendingAction.AwaitingTaskDescription:
                if (message.Text != "-")
                    payload.Description = message.Text;
                await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.AwaitingTaskPriority, SerializePayload(payload), ct);
                await ShowPriorityPickerAsync(chatId, ct);
                break;

            case PendingAction.AwaitingTaskDueDate:
                if (message.Text != "-")
                {
                    if (DateTimeOffset.TryParse(message.Text, out var dueDate))
                    {
                        payload.DueDate = dueDate;
                    }
                    else
                    {
                        await botClient.SendMessage(chatId,
                            "Неверный формат даты. " +
                            "Попробуйте ещё раз (ДД.ММ.ГГГГ) или отправьте \"-\" чтобы пропустить:",
                            cancellationToken: ct);
                        return;
                    }
                }

                await CreateTaskAsync(userId, chatId, payload, ct);
                break;
        }
    }

    private async Task ShowPriorityPickerAsync(long chatId, CancellationToken ct)
    {
        await botClient.SendMessage(
            chatId,
            "Выберите приоритет задачи:",
            replyMarkup: GetPriorityKeyboard(),
            cancellationToken: ct);
    }

    private InlineKeyboardMarkup GetPriorityKeyboard()
    {
        var buttons = Enum.GetValues<TaskPriority>()
            .Select(p => InlineKeyboardButton.WithCallbackData(
                GetPriorityLabel(p),
                $"tasks:priority:{p}"));

        return new InlineKeyboardMarkup(buttons.Chunk(2));
    }

    private static string GetPriorityLabel(TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "🟢 Низкий",
        TaskPriority.Medium => "🟡 Средний",
        TaskPriority.High => "🟠 Высокий",
        TaskPriority.Critical => "🔴 Критический",
        _ => priority.ToString(),
    };

    private async Task CreateTaskAsync(Guid userId, long chatId, TaskCreationPayload payload, CancellationToken ct)
    {
        var request = new CreateTaskRequest(
            payload.Title,
            payload.Description,
            payload.Priority is not null ? (TaskPriorityDto)payload.Priority : null,
            payload.DueDate);

        try
        {
            var task = await taskService.CreateAsync(userId, request, ct);

            await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);

            var message = $"✅ Задача создана!\n\n" +
                          $"📌 {task.Title}\n" +
                          $"📝 {task.Description ?? "—"}\n" +
                          $"⭐ Приоритет: {GetPriorityLabel((TaskPriority)task.Priority)}\n" +
                          $"📅 Дедлайн: {task.DueDate?.ToString("dd.MM.yyyy") ?? "—"}\n" +
                          $"🆔 ID: {task.Id:N}";

            await botClient.SendMessage(chatId, message, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create task for user {UserId}", userId);
            await botClient.SendMessage(
                chatId, $"❌ Ошибка при создании задачи: {ex.Message}", cancellationToken: ct);
            
            await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
        }
    }

    private static TaskCreationPayload DeserializePayload(string? payload)
        => string.IsNullOrWhiteSpace(payload)
            ? new TaskCreationPayload()
            : JsonSerializer.Deserialize<TaskCreationPayload>(payload) ?? new TaskCreationPayload();

    private static string SerializePayload(TaskCreationPayload payload)
        => JsonSerializer.Serialize(payload);

    private sealed class TaskCreationPayload
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTimeOffset? DueDate { get; set; }
    }
}