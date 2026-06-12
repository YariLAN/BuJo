using BuJo.Application.Accounting;
using BuJo.Application.Habits;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus.Habits;

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
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
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
        // PendingAction.AwaitingHabitName сохраняется — пользователь может ввести название повторно
    }
}
