using BuJo.Application.Habits;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Services.Habits;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages;

/// <summary>
/// Обрабатывает ввод названия новой привычки от пользователя
/// </summary>
public sealed class HabitNameInputHandler(
    IHabitService habitService,
    IHabitsMenuService habitsMenuService,
    ITelegramBotClient botClient,
    ILogger<HabitNameInputHandler> logger) : IPendingInputHandler
{
    public bool CanHandle(PendingAction action)
        => action is PendingAction.AwaitingHabitName;

    public async Task HandleAsync(Guid userId, long chatId, PendingAction action, Message message, CancellationToken ct)
    {
        var name = message.Text ?? string.Empty;

        await TryDeleteAsync(chatId, message.MessageId, ct);

        try
        {
            var habit = await habitService.CreateAsync(new CreateHabitCommand(userId, name), ct);
            await habitsMenuService.ShowCreatedAsync(userId, chatId, habit.Name, ct);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            await habitsMenuService.ShowValidationErrorAsync(userId, chatId, ex.Message, ct);
        }
    }

    private async Task TryDeleteAsync(long chatId, int messageId, CancellationToken ct)
    {
        try
        {
            await botClient.DeleteMessage(chatId, messageId, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete user message {MessageId} in chat {ChatId}", messageId, chatId);
        }
    }
}
