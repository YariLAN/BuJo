using BuJo.Application.Habits;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Services.Habits;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages.Habits;

/// <summary>
/// Обрабатывает выбор привычки для статистики по номеру из списка
/// </summary>
public sealed class StatsHabitSelectHandler(
    IHabitService habitService,
    IHabitsMenuService habitsMenuService,
    ILogger<StatsHabitSelectHandler> logger) : IPendingInputHandler
{
    public bool CanHandle(PendingAction action) => action == PendingAction.AwaitingStatsHabitSelect;

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
        await habitsMenuService.OpenStatsForHabitAsync(userId, chatId, selected.Id!.Value, ct);
    }
}