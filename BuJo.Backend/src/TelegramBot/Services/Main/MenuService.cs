using BuJo.Application.Accounting.Abstractions;
using BuJo.Application.Tasks.Abstractions;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus.Main;
using BuJo.TelegramBot.Menus.Settings;
using BuJo.TelegramBot.Menus.Tasks;
using BuJo.TelegramBot.Services.Tasks;
using Telegram.Bot;

namespace BuJo.TelegramBot.Services.Main;

internal sealed class MenuService(
    MenuRenderer renderer,
    ITaskService taskService,
    ITasksMenuService tasksMenuService,
    ITelegramBotClient botClient,
    IUserBotStateService userBotStateService) : IMenuService
{
    public async Task StartAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = MainMenuBuilder.Build();
        await renderer.RecreateAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenMainAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = MainMenuBuilder.Build();
        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenSettingsAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = SettingsMenuBuilder.Build();
        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenStubAsync(Guid userId, long chatId, string title, CancellationToken ct)
    {
        var view = StubMenuBuilder.Build(title);
        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenTasksAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var tasks = await taskService.GetTasksAsync(userId, ct);
        var view = TasksMenuBuilder.Build(tasks);
        await renderer.RecreateAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenTaskCreateAsync(Guid userId, long chatId, CancellationToken ct)
    {
        await tasksMenuService.OpenCreateAsync(userId, chatId, ct);
    }
}
