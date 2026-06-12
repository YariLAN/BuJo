using System.Text.Json;
using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using BuJo.Domain.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class TasksCallbackHandler(
    ITelegramBotClient botClient,
    IUserService userService,
    IUserBotStateService userBotStateService,
    ILogger<TasksCallbackHandler> logger) : CallbackHandlerBase(botClient, userService, logger)
{
    public override string Prefix => "tasks";

    protected override async Task HandleCallbackAsync(Guid userId, CallbackQuery callback, CancellationToken ct)
    {
        var parts = callback.Data!.Split(':');
        var action = parts[1];

        var chatId = callback.Message!.Chat.Id;

        switch (action)
        {
            case "create":
                await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.AwaitingTaskTitle, null, ct);
                await botClient.SendMessage(
                    chatId,
                    "📝 Введите название задачи:",
                    replyMarkup: new ForceReplyMarkup(),
                    cancellationToken: ct);
                break;

            case "priority":
                if (parts.Length < 3)
                {
                    logger.LogWarning("Invalid task_priority callback data: {Data}", callback.Data);
                    return;
                }

                var priority = Enum.Parse<TaskPriority>(parts[2], ignoreCase: true);
                var state = await userBotStateService.GetOrCreateAsync(userId, chatId, ct);
                var payload = DeserializePayload(state.PendingPayload);
                payload.Priority = priority;

                await userBotStateService.SetPendingActionAsync(
                    userId, chatId, PendingAction.AwaitingTaskDueDate, SerializePayload(payload), ct);

                await botClient.SendMessage(
                    chatId,
                    "Введите дедлайн в формате ДД.ММ.ГГГГ (или отправьте \"-\" чтобы пропустить):",
                    replyMarkup: new ForceReplyMarkup(),
                    cancellationToken: ct);
                break;

            default:
                logger.LogWarning("Unknown task callback action: {Action}", action);
                break;
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