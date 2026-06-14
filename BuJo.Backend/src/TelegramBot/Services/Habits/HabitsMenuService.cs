using BuJo.Application.Accounting;
using BuJo.Application.Habits;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus;
using BuJo.TelegramBot.Menus.Habits;
using BuJo.TelegramBot.Menus.Main;
using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Services.Habits;

internal sealed class HabitsMenuService(
    MenuRenderer renderer,
    IHabitService habitService,
    IUserBotStateService userBotStateService) : IHabitsMenuService
{
    public async Task OpenListAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var habits = await habitService.GetListAsync(new GetHabitsQuery(userId), ct);

        var view = HabitsListMenuBuilder.Build(habits);

        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.AwaitingHabitSelect, null, ct);
    }

    public async Task OpenCreatePromptAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = HabitCreatePromptBuilder.Build();

        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.AwaitingHabitName, payload: null, ct);
    }

    public async Task ShowCreatedAsync(Guid userId, long chatId, string habitName, CancellationToken ct)
    {
        var view = HabitCreatedMenuBuilder.Build(habitName);

        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task ShowValidationErrorAsync(Guid userId, long chatId, string error, CancellationToken ct)
    {
        var view = HabitCreatePromptBuilder.Build(error);
        await renderer.EditAsync(userId, chatId, view, ct);
    }

    public async Task OpenHabitAsync(Guid userId, long chatId, int inputMessageId, Guid habitId, CancellationToken ct)
    {
        var habit = await habitService.GetByIdAsync(habitId, ct);
        if (habit is null)
        {
            await ShowValidationErrorAsync(userId, chatId, "Привычка не найдена", ct);
            return;
        }

        await renderer.DeleteMessage(userId, chatId, inputMessageId, ct);
        await renderer.EditAsync(userId, chatId, HabitMenuBuilder.Build(habit.ToResponse()), ct);
        
        await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.None,
            payload: habitId.ToString(), ct);
    }

    public async Task OpenDatePickerAsync(Guid userId, long chatId, Guid habitId, CancellationToken ct)
    {
        await renderer.EditAsync(userId, chatId, DatePickerMenuBuilder.Build(), ct);
        
        await userBotStateService.SetPendingActionAsync(userId, chatId, PendingAction.None,
            payload: habitId.ToString(), ct);
    }

    public async Task MarkHabitAsync(
        Guid userId, 
        long chatId, 
        Guid habitId, 
        DateOnly date, 
        bool isCompleted, 
        bool fromChecklist, 
        CancellationToken ct)
    {
        try
        {
            var result = await habitService.LogAsync(new LogHabitCommand(userId, habitId, date, isCompleted), ct);
            await ShowMarkResultAsync(userId, chatId, result.HabitName ?? "Привычка", date, isCompleted,
                recreate: fromChecklist, ct);
        }
        catch (ArgumentException ex)
        {
            await ShowValidationErrorAsync(userId, chatId, ex.Message, ct);
        }
        catch (InvalidOperationException ex)
        {
            await ShowValidationErrorAsync(userId, chatId, ex.Message, ct);
        }
    }

    public async Task ShowMarkResultAsync(
        Guid userId, 
        long chatId, 
        string habitName, 
        DateOnly date, 
        bool isCompleted, 
        bool recreate, 
        CancellationToken ct)
    {
        var view = HabitMenuBuilder.BuildMarkResult(habitName, date, isCompleted);
        
        if (recreate)
            await renderer.RecreateAsync(userId, chatId, view, ct);
        else
            await renderer.EditAsync(userId, chatId, view, ct);
            
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenStatsSelectionAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var habits = await habitService.GetListAsync(new GetHabitsQuery(userId), ct);
        if (habits.Count == 0)
        {
            await ShowValidationErrorAsync(userId, chatId, 
                "У вас пока нет привычек. Сначала создайте хотя бы одну.", ct);
            
            return;
        }
        
        await renderer.EditAsync(userId, chatId, HabitsSelectionMenuBuilder.BuildForStats(habits), ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenStatsForHabitAsync(Guid userId, long chatId, Guid habitId, CancellationToken ct)
    {
        try
        {
            var stats = await habitService.GetStatsAsync(new GetHabitStatsQuery(userId, habitId), ct);
            var habit = await habitService.GetByIdAsync(habitId, ct);
            var habitName = habit?.Name ?? "Привычка";

            var view = HabitStatsBuilder.Build(stats, habitName);
            await renderer.EditAsync(userId, chatId, view, ct);
            await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
        }
        catch (InvalidOperationException ex)
        {
            await ShowValidationErrorAsync(userId, chatId, ex.Message, ct);
        }
    }

    public async Task ShowChecklistConfirmedAsync(Guid userId, long chatId, CancellationToken ct)
    {
        await renderer.EditAsync(userId, chatId, new MenuView(
            "✅ Спасибо! Все отметки сохранены.", 
            new InlineKeyboardMarkup(
                [[InlineKeyboardButton.WithCallbackData("🏠 Меню", MenuCallbacks.Main)]]
            )), 
            ct);
        
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }
}
