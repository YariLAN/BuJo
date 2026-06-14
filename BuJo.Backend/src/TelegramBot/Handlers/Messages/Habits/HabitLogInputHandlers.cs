using BuJo.Application.Accounting;
using BuJo.Application.Habits;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Services.Habits;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages.Habits;

/// <summary>
/// Обрабатывает выбор привычки по номеру из нумерованного списка
/// </summary>
public sealed class HabitSelectHandler(
    IHabitService habitService,
    IHabitsMenuService habitsMenuService,
    ILogger<HabitSelectHandler> logger) : IPendingInputHandler
{
    public bool CanHandle(PendingAction action) => action == PendingAction.AwaitingHabitSelect;

    public async Task HandleAsync(Guid userId, long chatId, PendingAction action, Message message, CancellationToken ct)
    {
        if (message.Text is null)
            return;

        if (!int.TryParse(message.Text.Trim(), out var index) || index < 1)
        {
            await habitsMenuService.ShowValidationErrorAsync(userId, chatId,
                "Пожалуйста, введите номер привычки из списка", ct);
            return;
        }

        var habits = await habitService.GetListAsync(new GetHabitsQuery(userId), ct);
        if (index > habits.Count)
        {
            await habitsMenuService.ShowValidationErrorAsync(userId, chatId,
                $"Введите число от 1 до {habits.Count}", ct);
            return;
        }

        var selected = habits[index - 1];
        await habitsMenuService.OpenHabitAsync(userId, chatId, message.Id, selected.Id!.Value, ct);
    }
}