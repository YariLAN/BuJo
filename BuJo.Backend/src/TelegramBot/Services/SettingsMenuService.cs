using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus;

namespace BuJo.TelegramBot.Services;

internal sealed class SettingsMenuService(
    MenuRenderer renderer,
    IUserService userService,
    IUserBotStateService userBotStateService) : ISettingsMenuService
{
    public async Task OpenRemindersAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var user = await userService.GetOrDefaultAsync(new GetUserQuery(userId), ct)
            ?? throw new InvalidOperationException($"Пользователь с Id={userId} не найден");

        var view = RemindersSettingsMenuBuilder.Build(user.ReminderMorningTime, user.ReminderEveningTime);

        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenTimePickerAsync(Guid userId, long chatId, ReminderKind kind, CancellationToken ct)
    {
        var view = TimePickerMenuBuilder.Build(kind);

        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenCustomTimePromptAsync(Guid userId, long chatId, ReminderKind kind, bool withError = false, CancellationToken ct = default)
    {
        var view = CustomTimePromptBuilder.Build(kind, withError);

        await renderer.EditAsync(userId, chatId, view, ct);

        var pendingAction = kind switch
        {
            ReminderKind.Morning => PendingAction.AwaitingMorningTime,
            ReminderKind.Evening => PendingAction.AwaitingEveningTime,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        await userBotStateService.SetPendingActionAsync(userId, chatId, pendingAction, payload: null, ct);
    }
}
